using DotNetty.Common.Utilities;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions
{
    public class Constants
    {
        public static readonly int RegionSize = 8;
        public static readonly int ArchiveCount = 9;
        public static readonly AttributeKey<Player> PlayerAttributeKey = AttributeKey<Player>.ValueOf("Player");
    }
}
