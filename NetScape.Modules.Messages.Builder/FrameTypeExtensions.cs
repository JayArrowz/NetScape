namespace NetScape.Modules.Messages.Builder
{
    public static class FrameTypeExtensions
    {
        public static int GetBytes(this FrameType frameType)
        {
            switch (frameType)
            {
                case FrameType.VariableByte:
                    return sizeof(byte);
                case FrameType.VariableShort:
                    return sizeof(short);
                case FrameType.Fixed:
                case FrameType.Raw:
                default:
                    return 0;
            }
        }
    }
}
