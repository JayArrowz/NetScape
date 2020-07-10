using ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Model;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Numerics;
using System.Threading.Tasks;
using ASPNetScape.Abstractions;
using Serilog;
using ASPNetScape.Abstractions.Extensions;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Login
{
    public class WorldLoginHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ILogger _logger;
        private readonly LoginType _loginType;
        private readonly short _packetSize;
        private readonly int _clientVersion;

        public WorldLoginHandler(ILogger logger, LoginType loginType, short packetSize, int clientVersion)
        {
            _logger = logger;
            _loginType = loginType;
            _packetSize = packetSize;
            _clientVersion = clientVersion;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            msg.ReadByte();

            var rsaBlockSize = msg.ReadUnsignedShort();
            if(rsaBlockSize > msg.ReadableBytes)
            {
                SendResponse(ctx, LoginResponse.InvalidLogin).ConfigureAwait(false);
                return;
            }

            byte[] rsaBlockData = new byte[rsaBlockSize];
            msg.ReadBytes(rsaBlockData);
            var rsaBlockBuffer = Unpooled.WrappedBuffer(BigInteger
                .ModPow(new BigInteger(rsaBlockData), Constants.RSA_EXPONENT, Constants.RSA_MODULUS).ToByteArray());
            if (rsaBlockBuffer.ReadByte() != 10)
            {
                SendResponse(ctx, LoginResponse.InvalidLogin).ConfigureAwait(false);
                return;
            }

            var isaacKeys = new int[4];
            for (int i = 0; i < isaacKeys.Length; i++)
            {
                isaacKeys[i] = rsaBlockBuffer.ReadInt();
            }

            if(rsaBlockBuffer.ReadLong() != 0L)
            {
                _logger.Information("Invalid Login RSA Block buffer.");
                SendResponse(ctx, LoginResponse.InvalidLogin).ConfigureAwait(false);
                return;
            }

            var password = rsaBlockBuffer.ReadString();

            //Unknown
            rsaBlockBuffer.ReadLong(); 
            rsaBlockBuffer.ReadLong();
            rsaBlockBuffer.ReadLong();

        }

        private async Task SendResponse(IChannelHandlerContext ctx, LoginResponse response, bool closeChannel = true)
        {
            var buffer = Unpooled.Buffer(1);
            buffer.WriteByte((int) response);
            await ctx.WriteAndFlushAsync(buffer);

            if(closeChannel)
            {
                await ctx.Channel.CloseAsync();
            }

            ctx.Channel.Pipeline.Remove(this);
        }
    }
}
