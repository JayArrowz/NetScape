using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Messages;
using NetScape.Abstractions.Util;
using static NetScape.Abstractions.Model.Messages.MessageFrame;

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

            if (Cipher != null)
            {
                opcode += (int) Cipher.NextInt();
            }

            output.WriteByte(opcode);
            if (type == MessageFrame.MessageType.VariableByte)
                output.WriteByte(payload.ReadableBytes);
            else if (type == MessageFrame.MessageType.VariableShort)
                output.WriteShort(payload.ReadableBytes);

            output.WriteBytes(payload);
        }
    }
}
