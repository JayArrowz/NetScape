using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System.Collections.Generic;
namespace NetScape.Modules.Messages
{
    public class MessageHeaderDecoder : ByteToMessageDecoder
    {
        private readonly ILogger _logger;

        public MessageHeaderDecoder(ILogger logger)
        {
            _logger = logger;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var opcode = input.ReadByte();
            _logger.Debug("Decoding Opcode: {0} from {1}", opcode, context.Channel.RemoteAddress);
        }
    }
}
