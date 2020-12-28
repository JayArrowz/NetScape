using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Abstractions.Model.World.Updating.Blocks;
using NetScape.Abstractions.Util;

namespace NetScape.Abstractions.Model.World.Updating
{
    public abstract class SynchronizationBlock
    {
        public static SynchronizationBlock CreateAnimationBlock(Animation animation)
        {
            return new AnimationBlock(animation);
        }

        public static SynchronizationBlock CreateAppearanceBlock(Player player)
        {
            int combat = 3;
            return new AppearanceBlock(TextUtil.NameToLong(player.Username), player.Appearance, combat, 0, 0, false);

        }
    }
}
