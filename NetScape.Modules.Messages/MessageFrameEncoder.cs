using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Util;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Modules.Messages
{
    public class MessageFrameEncoder : MessageToByteEncoder<MessageFrame>, ICipheredHandler
    {
        public IsaacRandom Cipher { get; set; }

        protected override void Encode(IChannelHandlerContext context, MessageFrame frame, IByteBuffer output)
        {
            var type = frame.Type;
            IByteBuffer payload = frame.Payload;
            int opcode = frame.Id;

            opcode += (int) (Cipher?.NextInt() ?? 0);
            
            output.WriteByte(opcode);
            if (type == FrameType.VariableByte)
                output.WriteByte(payload.ReadableBytes);
            else if (type == FrameType.VariableShort)
                output.WriteShort(payload.ReadableBytes);
            output.WriteBytes(payload);
        }
    }
}
