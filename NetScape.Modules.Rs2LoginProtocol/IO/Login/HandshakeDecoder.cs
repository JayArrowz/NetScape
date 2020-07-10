using NetScape.Abstractions.Model.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System.Collections.Generic;

namespace NetScape.Modules.LoginProtocolThreeOneSeven.IO.Login
{
    public class HandshakeDecoder : ByteToMessageDecoder
    {
        private readonly ILogger _logger;

        public HandshakeDecoder(ILogger logger)
        {
            _logger = logger;
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            if (!buffer.IsReadable())
            {
                return;
            }

            var id = (int) buffer.ReadByte();
            var handshakeType = (HandshakeType)id;
            _logger.Debug($"Incoming Handshake Decoder Opcode: {id} Type: {handshakeType}");
            switch (id)
            {
                case (int)HandshakeType.ServiceGame:
                    ctx.Channel.Pipeline.AddLast(nameof(LoginEncoder), new LoginEncoder());
                    ctx.Channel.Pipeline.AddAfter(nameof(LoginEncoder), nameof(LoginDecoder), new LoginDecoder(_logger));
                    break;

                //case HandshakeType.SERVICE_UPDATE:
                //    ctx.Channel.Pipeline.AddFirst("updateEncoder", new UpdateEncoder());
                //    ctx.Channel.Pipeline.AddBefore("handler", "updateDecoder", new UpdateDecoder());

                //    var buf = ctx.Allocator.Buffer(8).WriteLong(0);
                //    ctx.Channel.WriteAndFlushAsync(buf);
                //    break;

                default:
                    _logger.Information($"Unexpected handshake request received: {id}");
                    return;
            }

            ctx.Channel.Pipeline.Remove(this);
            output.Add(handshakeType);
        }
    }
}
