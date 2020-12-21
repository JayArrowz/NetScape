namespace NetScape.Abstractions.Model.Messages
{
    public enum MessageType
    {
        Byte = sizeof(byte),
        Short = sizeof(short),
        TriByte = 3,
        Int = sizeof(int),
        Long = sizeof(long)
    }
}