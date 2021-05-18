using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.FourSevenFour.LoginProtocol.Handlers
{
    public class HandshakeDecoder : ByteToMessageDecoder
    {
        private readonly ILogger _logger;
        private readonly LoginEncoder _loginEncoder;
        private readonly LoginDecoder _loginDecoder;
        private readonly JS5Decoder _jS5Decoder;
        private readonly JS5Encoder _jS5Encoder;

        public HandshakeDecoder(ILogger logger, LoginEncoder loginEncoder, LoginDecoder loginDecoder,
            JS5Decoder jS5Decoder, JS5Encoder jS5Encoder)
        {
            _jS5Decoder = jS5Decoder;
            _jS5Encoder = jS5Encoder;
            _logger = logger;
            _loginEncoder = loginEncoder;
            _loginDecoder = loginDecoder;
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (!buffer.IsReadable())
            {
                return;
            }

            var id = buffer.ReadByte();
            var handshakeType = (HandshakeType)id;
            _logger.Debug("Incoming Handshake Decoder Opcode: {0} Type: {1}", id, handshakeType);
            switch (handshakeType)
            {
                case HandshakeType.ServiceGame:
                    ctx.Channel.Pipeline.AddLast(nameof(LoginEncoder), _loginEncoder);
                    ctx.Channel.Pipeline.AddAfter(nameof(LoginEncoder), nameof(LoginDecoder), _loginDecoder);
                    break;

                case HandshakeType.ServiceUpdate:
                    int version = buffer.ReadInt();
                    ctx.Channel.Pipeline.AddLast(nameof(JS5Decoder), _jS5Decoder);
                    ctx.Channel.Pipeline.AddLast(nameof(JS5Encoder), _jS5Encoder);

                    //Really should do version checking
                    _ = ctx.Channel.WriteAndFlushAsync(ctx.Allocator.Buffer(1).WriteByte((int)FourSevenFourLoginStatus.StatusExchangeData));
                    break;

                default:
                    _logger.Information("Unexpected handshake request received: {0}", id);
                    return;
            }

            ctx.Channel.Pipeline.Remove(this);
            output.Add(handshakeType);
        }
    }
}
