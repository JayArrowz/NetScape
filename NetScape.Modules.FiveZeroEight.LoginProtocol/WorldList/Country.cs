using System;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.WorldList
{
    public class Country
    {
        public const int FLAG_UK = 77;
        public const int FLAG_USA = 225;
        public const int FLAG_CANADA = 38;
        public const int FLAG_NETHERLANDS = 161;
        public const int FLAG_AUSTRALIA = 16;
        public const int FLAG_SWEDEN = 191;
        public const int FLAG_FINLAND = 69;
        public const int FLAG_IRELAND = 101;
        public const int FLAG_BELGIUM = 22;
        public const int FLAG_NORWAY = 162;
        public const int FLAG_DENMARK = 58;
        public const int FLAG_BRAZIL = 31;
        public const int FLAG_MEXICO = 152;

        public int Flag { get; }
        public string Name { get; }

        public Country(int flag, string name)
        {
            Flag = flag;
            Name = name;
        }
    }
}
