namespace NetScape.Modules.FiveZeroEight.LoginProtocol.WorldList
{
    public class World
    {
        public const int FLAG_MEMBERS = 0x1;
        public const int FLAG_QUICK_CHAT = 0x2;
        public const int FLAG_PVP = 0x4;
        public const int FLAG_LOOT_SHARE = 0x8;
        public const int FLAG_HIGHLIGHT = 0x10;

        public int Id { get; }
        public int Flags { get; }
        public int Country { get; }
        public string Activity { get; }
        public string Ip { get; }

        public World(int id, int flags, int country, string activity, string ip)
        {
            Id = id;
            Flags = flags;
            Country = country;
            Activity = activity;
            Ip = ip;
        }
    }
}
