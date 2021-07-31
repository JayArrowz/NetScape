namespace NetScape.Modules.FourSevenFour.LoginProtocol
{
    public class JS5Request
    {
        public int Index { get; }
        public int File { get; }
        public bool Priority { get; }
        public int EncryptionKey { get; }
        public JS5Request(int index, int file, bool priority, int encryptionKey)
        {
            Index = index;
            File = file;
            Priority = priority;
            EncryptionKey = encryptionKey;
        }
    }
}
