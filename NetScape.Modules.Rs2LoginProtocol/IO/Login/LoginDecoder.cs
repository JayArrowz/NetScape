using NetScape.Abstractions;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.IO;
using NetScape.Abstractions.IO.Login;
using NetScape.Abstractions.Model.IO.Login;
using NetScape.Modules.LoginProtocolThreeOneSeven.IO.Model;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;

namespace NetScape.Modules.LoginProtocolThreeOneSeven.IO.Login
{
    /**
     * @author Graham
     * Modified by JayArrowz
     */
    public class LoginDecoder : StatefulFrameDecoder<LoginDecoderState>
    {
        private static readonly Random Random = new Random();

        /**
         * The login packet length.
         */
        private int _loginLength;

        /**
         * The reconnecting flag.
         */
        private bool _reconnecting;

        /**
         * The server-side session key.
         */
        private long _serverSeed;

        /**
         * The username hash.
         */
        private int _usernameHash;

        private readonly ILogger _logger;

        public LoginDecoder(ILogger logger) : base(LoginDecoderState.LoginHandshake)
        {
            _logger = logger;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output, LoginDecoderState state)
        {
            _logger.Debug($"Login Request Recieved: {state}");
            switch (state)
            {
                case LoginDecoderState.LoginHandshake:
                    DecodeHandshake(context, input, output);
                    break;
                case LoginDecoderState.LoginHeader:
                    DecodeHeader(context, input, output);
                    break;
                case LoginDecoderState.LoginPayload:
                    DecodePayload(context, input, output);
                    break;
                default:
                    throw new InvalidOperationException("Invalid login decoder state: " + state);
            }
        }

        /**
         * Decodes in the handshake state.
         *
         * @param ctx The channel handler context.
         * @param buffer The buffer.
         * @param out The {@link List} of objects to pass forward through the pipeline.
         */
        private void DecodeHandshake(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (buffer.IsReadable())
            {
                _usernameHash = buffer.ReadByte();
                _serverSeed = Random.NextLong();

                var response = ctx.Allocator.Buffer(17);
                response.WriteByte((int)LoginStatus.StatusExchangeData);
                response.WriteLong(0);
                response.WriteLong(_serverSeed);
                ctx.Channel.WriteAndFlushAsync(response);

                SetState(LoginDecoderState.LoginHeader);
            }
        }

        /**
         * Decodes in the header state.
         *
         * @param ctx The channel handler context.
         * @param buffer The buffer.
         * @param out The {@link List} of objects to pass forward through the pipeline.
         */
        private void DecodeHeader(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (buffer.ReadableBytes >= 2)
            {
                var type = buffer.ReadByte();

                if (type != (int)LoginStatus.TypeStandard && type != (int)LoginStatus.TypeReconnection)
                {
                    _logger.Information("Failed to decode login header.");
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                _reconnecting = type == (int)LoginStatus.TypeReconnection;
                _loginLength = buffer.ReadByte();
                SetState(LoginDecoderState.LoginPayload);
            }
        }

        /**
         * Decodes in the payload state.
         *
         * @param ctx The channel handler context.
         * @param buffer The buffer.
         * @param out The {@link List} of objects to pass forward through the pipeline.
         */
        private void DecodePayload(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (buffer.ReadableBytes >= _loginLength)
            {
                IByteBuffer payload = buffer.ReadBytes(_loginLength);
                var version = 255 - payload.ReadByte();

                var release = payload.ReadShort();

                var memoryStatus = payload.ReadByte();
                if (memoryStatus != 0 && memoryStatus != 1)
                {
                    _logger.Information("Login memoryStatus (" + memoryStatus + ") not in expected range of [0, 1].");
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                var lowMemory = memoryStatus == 1;

                var crcs = new int[Constants.ArchiveCount];
                for (var index = 0; index < Constants.ArchiveCount; index++)
                {
                    crcs[index] = payload.ReadInt();
                }

                var length = payload.ReadByte();
                if (length != _loginLength - 41)
                {
                    _logger.Information("Login packet unexpected length (" + length + ")");
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                /*
                var secureBytes = payload.ReadBytes(length);
                var value = new BigInteger(secureBytes.Array.Take(length).ToArray());
                *
                 * RSA?
                value = BigInteger.ModPow(value, Constants.RSA_MODULUS, Constants.RSA_EXPONENT);
                var secureBuffer = Unpooled.WrappedBuffer(value.ToByteArray());
                */


                var id = payload.ReadByte();
                if (id != 10)
                {
                    _logger.Information("Unable to read id from secure payload.");
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }


                var clientSeed = payload.ReadLong();
                var reportedSeed = payload.ReadLong();
                if (reportedSeed != _serverSeed)
                {
                    _logger.Information("Reported seed differed from server seed.");
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                var uid = payload.ReadInt();
                var username = payload.ReadString();
                var password = payload.ReadString();
                var socketAddress = (IPEndPoint)ctx.Channel.RemoteAddress;
                var hostAddress = socketAddress.Address.ToString();

                if (password.Length < 6 || password.Length > 20 || string.IsNullOrEmpty(username) || username.Length > 12)
                {
                    _logger.Information("Username ('" + username + "') or password did not pass validation.");
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                var seed = new int[4];
                seed[0] = (int)(clientSeed >> 32);
                seed[1] = (int)clientSeed;
                seed[2] = (int)(_serverSeed >> 32);
                seed[3] = (int)_serverSeed;

                var decodingRandom = new IsaacRandom(seed);
                for (int index = 0; index < seed.Length; index++)
                {
                    seed[index] += 50;
                }

                var encodingRandom = new IsaacRandom(seed);

                var credentials = new PlayerCredentials(username, password, _usernameHash, uid, hostAddress);
                var randomPair = new IsaacRandomPair(encodingRandom, decodingRandom);
                output.Add(new LoginRequest(credentials, randomPair, _reconnecting, lowMemory, release, crcs, version));
            }
        }

        /**
         * Writes a response code to the client and closes the current channel.
         *
         * @param ctx The context of the channel handler.
         * @param response The response code to write.
         */
        private void WriteResponseCode(IChannelHandlerContext ctx, LoginStatus response)
        {
            var buffer = ctx.Allocator.Buffer(sizeof(byte));
            buffer.WriteByte((int)response);
            ctx.WriteAndFlushAsync(buffer);

            if (response != LoginStatus.StatusOk)
            {
                ctx.DisconnectAsync();
            }
        }
    }
}
