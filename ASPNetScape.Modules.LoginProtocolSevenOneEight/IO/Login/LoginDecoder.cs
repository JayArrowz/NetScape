using ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Model;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Login
{
    public class LoginDecoder : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ILogger _logger;

        public LoginDecoder(ILogger logger)
        {
            _logger = logger;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            var loginType = (LoginType) msg.ReadByte();
            var packetSize = msg.ReadShort();
            var clientVersion = msg.ReadInt();

            switch(loginType)
            { 
                //TODO Lobby Implementation
                case LoginType.LOBBY:
                case LoginType.WORLD:
                    ctx.Channel.Pipeline.AddAfter(nameof(LoginDecoder), nameof(WorldLoginHandler), new WorldLoginHandler(_logger, loginType, packetSize, clientVersion));
                    ctx.FireChannelRead(msg);
                    break;

                case LoginType.UNKNOWN:
                    _logger.Information($"Unknown login type. pkSize: {packetSize} clientVersion: {clientVersion}");
                    ctx.CloseAsync().ConfigureAwait(false);
                    break;
            }

            ctx.Channel.Pipeline.Remove(this);
        }
    }
}
