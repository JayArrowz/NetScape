using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Modules.Messages
{
    public class MessageFrameEncoder : MessageToByteEncoder<MessageFrame>, ICipherAwareHandler, IPlayerAwareHandler
    {
        public IsaacRandomPair CipherPair { get; set; }
        public Player Player { get; set; }

        protected override void Encode(IChannelHandlerContext context, MessageFrame frame, IByteBuffer output)
        {
            var type = frame.Type;
            IByteBuffer payload = frame.Payload;
            int opcode = frame.Id;

            int? isaacValue = CipherPair?.EncodingRandom?.NextInt() ?? null;
            opcode = isaacValue.HasValue ? opcode + isaacValue.Value & 0xFF : opcode & 0XFF;

            output.WriteByte(opcode);
            if (type == FrameType.VariableByte)
                output.WriteByte(payload.ReadableBytes);
            else if (type == FrameType.VariableShort)
                output.WriteShort(payload.ReadableBytes);
            output.WriteBytes(payload);
        }
    }
}
