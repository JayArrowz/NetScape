using System;

namespace NetScape.Modules.Messages.Builder
{
    public static class FrameTypeExtensions
    {
        public static int GetBytes(this FrameType frameType, DotNetty.Buffers.IByteBuffer input)
        {
            switch (frameType)
            {
                case FrameType.ReadAll:
                    return input.ReadableBytes;
                case FrameType.VariableByte:
                    return input.ReadByte();
                case FrameType.VariableShort:
                    return input.ReadUnsignedShort();
                default:
                    throw new ArgumentException("Invalid frameType input argument");
            }
        }
    }
}
