using DotNetty.Buffers;

namespace NetScape.Abstractions.Model.Messages
{
    /**
    * @author Graham
    */
    public class MessageFrame
    {
        public int Id { get; }
        public MessageType Type { get; }
        public IByteBuffer Payload { get; }

        public MessageFrame(int opcode, MessageType type, IByteBuffer payload)
        {
            Id = opcode;
            Type = type;
            Payload = payload;
        }

        public enum MessageType
        {
            Raw, Fixed, VariableByte, VariableShort
        }

    }
}
