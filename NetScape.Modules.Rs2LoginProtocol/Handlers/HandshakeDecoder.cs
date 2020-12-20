using NetScape.Abstractions.Model.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System.Collections.Generic;

namespace NetScape.Modules.LoginProtocol.Handlers
{
    public class HandshakeDecoder : ByteToMessageDecoder
    {
        private readonly ILogger _logger;
        private readonly LoginEncoder _loginEncoder;
        private readonly LoginDecoder _loginDecoder;

        public HandshakeDecoder(ILogger logger, LoginEncoder loginEncoder, LoginDecoder loginDecoder)
        {
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

                //case HandshakeType.SERVICE_UPDATE:
                //    ctx.Channel.Pipeline.AddFirst("updateEncoder", new UpdateEncoder());
                //    ctx.Channel.Pipeline.AddBefore("handler", "updateDecoder", new UpdateDecoder());

                //    var buf = ctx.Allocator.Buffer(8).WriteLong(0);
                //    ctx.Channel.WriteAndFlushAsync(buf);
                //    break;

                default:
                    _logger.Information("Unexpected handshake request received: {0}", id);
                    return;
            }

            ctx.Channel.Pipeline.Remove(this);
            output.Add(handshakeType);
        }
    }
}
