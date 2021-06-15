using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using NetScape.Abstractions;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.World;
using NetScape.Abstractions.IO;
using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Login;
using NetScape.Abstractions.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers
{
    /// <summary>
    /// The Game Login decoder
    /// </summary>
    /// <seealso cref="NetScape.Abstractions.IO.StatefulFrameDecoder{NetScape.Abstractions.Model.Login.LoginDecoderState}" />
    public class LoginDecoder : StatefulFrameDecoder<LoginDecoderState>
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// The login packet length
        /// </summary>
        private int _loginLength;

        /// <summary>
        /// The reconnecting flag
        /// </summary>
        private bool _reconnecting;

        /// <summary>
        /// The server side session key
        /// </summary>
        private long _serverSeed;

        /// <summary>
        /// The username hash
        /// </summary>
        private int _usernameHash;
        private readonly ILogger _logger;
        private readonly ILoginProcessor<FiveZeroEightLoginRequest, FiveZeroEightLoginResponse> _loginProcessor;
        private readonly IMessageProvider _gameMessageProvider;
        private readonly IWorld _world;
        private readonly IPlayerInitializer _playerInitializer;
        public LoginDecoder(ILogger logger, ILoginProcessor<FiveZeroEightLoginRequest, FiveZeroEightLoginResponse> loginProcessor, IMessageProvider gameMessageProvider, IWorld world, IPlayerInitializer playerInitializer) : base(LoginDecoderState.LoginHandshake)
        {
            _logger = logger;
            _gameMessageProvider = gameMessageProvider;
            _loginProcessor = loginProcessor;
            _world = world;
            _playerInitializer = playerInitializer;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output, LoginDecoderState state)
        {
            _logger.Debug("Login Request Recieved: {0} IP: {1}", state, context.Channel.RemoteAddress);
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

        /// <summary>
        /// Decodes the handshake state.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="output">The output.</param>
        private void DecodeHandshake(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (buffer.IsReadable())
            {
                _usernameHash = buffer.ReadByte();
                _serverSeed = Random.NextLong();

                var response = ctx.Allocator.Buffer(17);
                response.WriteByte((int)FiveZeroEightLoginStatus.StatusExchangeData);
                response.WriteLong(_serverSeed);
                ctx.Channel.WriteAndFlushAsync(response);
                SetState(LoginDecoderState.LoginHeader);
            }
        }

        /// <summary>
        /// Decodes the header state.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="output">The output.</param>
        private void DecodeHeader(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (buffer.ReadableBytes >= 2)
            {
                var type = buffer.ReadByte();

                if (type != (int)FiveZeroEightLoginStatus.TypeStandard && type != (int)FiveZeroEightLoginStatus.TypeReconnection)
                {
                    _logger.Information("Failed to decode login header.");
                    WriteResponseCode(ctx, FiveZeroEightLoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                _reconnecting = type == (int)FiveZeroEightLoginStatus.TypeReconnection;
                _loginLength = buffer.ReadByte();
                SetState(LoginDecoderState.LoginPayload);
            }
        }

        /// <summary>
        /// Decodes the payload state.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="output">The output.</param>
        private void DecodePayload(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (buffer.ReadableBytes >= _loginLength)
            {
                IByteBuffer payload = buffer.ReadBytes(_loginLength);
                var version = payload.ReadInt();

                var memoryStatus = payload.ReadByte();
                if (memoryStatus != 0 && memoryStatus != 1)
                {
                    _logger.Information("Login memoryStatus ({0}) not in expected range of [0, 1].", memoryStatus);
                    WriteResponseCode(ctx, FiveZeroEightLoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                var lowMemory = memoryStatus == 1;

                payload.SkipBytes(24);
                payload.SkipBytes(64);

                /*var crcs = new int[Constants.ArchiveCount];
                for (var index = 0; index < Constants.ArchiveCount; index++)
                {
                    crcs[index] = payload.ReadInt();
                }*/

               /* var length = payload.ReadByte();
                if (length != _loginLength - 41)
                {
                    _logger.Information("Login packet unexpected length ({0})", length);
                    WriteResponseCode(ctx, LoginStatus.StatusLoginServerRejectedSession);
                    return;
                }*/

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
                    WriteResponseCode(ctx, FiveZeroEightLoginStatus.StatusLoginServerRejectedSession);
                    return;
                }


                var clientSeed = payload.ReadLong();
                var reportedSeed = payload.ReadLong();
                if (reportedSeed != _serverSeed)
                {
                    _logger.Information("Reported seed differed from server seed.");
                    WriteResponseCode(ctx, FiveZeroEightLoginStatus.StatusLoginServerRejectedSession);
                    return;
                }

                var username = TextUtil.LongToName(payload.ReadLong());
                var password = payload.ReadString(0);
                var socketAddress = (IPEndPoint)ctx.Channel.RemoteAddress;
                var hostAddress = socketAddress.Address.ToString();

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

                var credentials = new PlayerCredentials
                {
                    Username = username,
                    Password = password,
                    EncodedUsername = _usernameHash,
                    Uid = -1,
                    HostAddress = hostAddress,
                };

                var randomPair = new IsaacRandomPair(encodingRandom, decodingRandom);
                var request = new FiveZeroEightLoginRequest
                {
                    Credentials = credentials,
                    RandomPair = randomPair,
                    Reconnecting = _reconnecting,
                    LowMemory = lowMemory,
                    ReleaseNumber = version,
                    ArchiveCrcs = null,
                    ClientVersion = version,
                    OnResult = (loginTask) => WriteProcessorResponseAsync(loginTask, ctx, randomPair)
                };

                _loginProcessor.Enqueue(request);
            }
        }

        private async Task WriteProcessorResponseAsync(FiveZeroEightLoginResponse loginResult, IChannelHandlerContext ctx, IsaacRandomPair randomPair)
        {
            try
            {
                await ctx.WriteAndFlushAsync(loginResult);
                HandleLoginProcessorResponse(loginResult.Player, loginResult.Status, ctx, randomPair);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, nameof(WriteProcessorResponseAsync));
                await ctx.CloseAsync();
            }
        }

        /// <summary>
        /// Writes a response code to the client and closes the current channel.
        /// </summary>
        /// <param name="ctx">The ctx.</param>
        /// <param name="response">The response.</param>
        private void WriteResponseCode(IChannelHandlerContext ctx, FiveZeroEightLoginStatus response)
        {
            var buffer = ctx.Allocator.Buffer(sizeof(byte));
            buffer.WriteByte((int)response);
            ctx.WriteAndFlushAsync(buffer);
            HandleLoginProcessorResponse(null, response, ctx, null);
        }

        private void HandleLoginProcessorResponse(Player player, FiveZeroEightLoginStatus response, IChannelHandlerContext ctx, IsaacRandomPair randomPair)
        {
            if (response != FiveZeroEightLoginStatus.StatusOk)
            {
                ctx.CloseAsync();
            }
            else
            {
                if (player == null)
                {
                    ctx.CloseAsync();
                    throw new InvalidOperationException("Cannot initialize player is null");
                }

                ctx.Channel.Pipeline.Remove(nameof(LoginEncoder));
                ctx.Channel.Pipeline.Remove(nameof(LoginDecoder));
                var gameMessageHandlers = _gameMessageProvider.Provide();

                foreach (var gameMessageHandler in gameMessageHandlers)
                {
                    /*if (gameMessageHandler is ICipherAwareHandler)
                    {
                        ((ICipherAwareHandler)gameMessageHandler).CipherPair = randomPair;
                    }*/

                    if (gameMessageHandler is IPlayerAwareHandler)
                    {
                        ((IPlayerAwareHandler)gameMessageHandler).Player = player;
                    }
                }
                ctx.Channel.Pipeline.AddLast(gameMessageHandlers);
                ctx.GetAttribute(Constants.PlayerAttributeKey).SetIfAbsent(player);
                player.ChannelHandlerContext = ctx;
                _world.Add(player);
                if (!_reconnecting)
                {
                    _ = _playerInitializer.InitializeAsync(player);
                }
                else
                {
                    player.UpdateAppearance();
                }
            }
        }
    }
}
