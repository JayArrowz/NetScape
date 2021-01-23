using DotNetty.Buffers;
using System;

namespace NetScape.Modules.Messages.Builder
{
    /**
    * @author Graham
    */
    [Serializable]
    public class MessageFrame
    {
        public int Id { get; }
        public FrameType Type { get; }
        public IByteBuffer Payload { get; }

        public MessageFrame(int opcode, FrameType type, IByteBuffer payload)
        {
            Id = opcode;
            Type = type;
            Payload = payload;
        }

    }
}
