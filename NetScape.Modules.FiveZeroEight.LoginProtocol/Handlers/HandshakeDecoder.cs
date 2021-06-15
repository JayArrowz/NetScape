using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers
{
    public class HandshakeDecoder : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ILogger _logger;
        private readonly LoginEncoder _loginEncoder;
        private readonly LoginDecoder _loginDecoder;
        private readonly JS5Decoder _jS5Decoder;
        private readonly JS5Encoder _jS5Encoder;
        private readonly WorldListDecoder _worldListDecoder;
        private readonly WorldListEncoder _worldListEncoder;

        public HandshakeDecoder(ILogger logger, LoginEncoder loginEncoder, LoginDecoder loginDecoder,
            JS5Decoder jS5Decoder, JS5Encoder jS5Encoder, WorldListDecoder worldListDecoder, WorldListEncoder worldListEncoder)
        {
            _jS5Decoder = jS5Decoder;
            _jS5Encoder = jS5Encoder;
            _worldListDecoder = worldListDecoder;
            _worldListEncoder = worldListEncoder;
            _logger = logger;
            _loginEncoder = loginEncoder;
            _loginDecoder = loginDecoder;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer buffer)
        {
            if (!buffer.IsReadable())
            {
                return;
            }

            var id = buffer.ReadByte();
            var handshakeType = (HandshakeType)id;
            _logger.Debug("Incoming Handshake Decoder Opcode: {0} Type: {1}", id, handshakeType);

            var pipeline = ctx.Channel.Pipeline;
            var retain = handshakeType == HandshakeType.ServiceWorldList || handshakeType == HandshakeType.ServiceUpdate;
            pipeline.Remove(this);
            switch (handshakeType)
            {
                case HandshakeType.ServiceGame:
                    ctx.Channel.Pipeline.AddLast(nameof(LoginEncoder), _loginEncoder);
                    ctx.Channel.Pipeline.AddAfter(nameof(LoginEncoder), nameof(LoginDecoder), _loginDecoder);
                    break;

                case HandshakeType.ServiceWorldList:
                    ctx.Channel.Pipeline.AddLast(nameof(WorldListDecoder), _worldListDecoder);
                    ctx.Channel.Pipeline.AddLast(nameof(WorldListEncoder), _worldListEncoder);
                    break;
                case HandshakeType.ServiceUpdate:
                    int version = buffer.ReadInt();
                    ctx.Channel.Pipeline.AddLast(nameof(JS5Decoder), _jS5Decoder);
                    ctx.Channel.Pipeline.AddLast(nameof(JS5Encoder), _jS5Encoder);
                    //Really should do version checking
                    _ = ctx.Channel.WriteAndFlushAsync(ctx.Allocator.Buffer(1).WriteByte((int)FiveZeroEightLoginStatus.StatusExchangeData));
                    break;
                default:
                    _logger.Information("Unexpected handshake request received: {0}", id);
                    return;
            }

            if (retain)
            {
                if (buffer.IsReadable())
                {
                    buffer.Retain();
                    ctx.Channel.Pipeline.FireChannelRead(buffer);
                }
            }
        }
    }
}
