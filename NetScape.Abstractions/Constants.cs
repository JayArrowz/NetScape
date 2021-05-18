using DotNetty.Common.Utilities;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions
{
    public class Constants
    {
        public static int RegionSize { get; } = 8;
        public static AttributeKey<Player> PlayerAttributeKey { get; } = AttributeKey<Player>.ValueOf("Player");
        public static Position HomePosition { get; } = new Position(3333, 3333, 0);
    }
}
