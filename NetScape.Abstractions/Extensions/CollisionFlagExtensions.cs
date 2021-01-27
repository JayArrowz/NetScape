using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region.Collision;

namespace NetScape.Abstractions.Extensions
{
    public static class CollisionFlagExtensions
    {
        /**
		 * Returns an array of CollisionFlags that indicate if a Mob can be positioned on a tile.
		 *
		 * @return The array of CollisionFlags.
		 */
        public static CollisionFlag[] Mobs()
        {
            return new CollisionFlag[] {
                CollisionFlag.Mob_North_West,
                CollisionFlag.Mob_North,
                CollisionFlag.Mob_North_East,
                CollisionFlag.Mob_West,
                CollisionFlag.Mob_East,
                CollisionFlag.Mob_South_West,
                CollisionFlag.Mob_South,
                CollisionFlag.Mob_South_East,
            };
        }

        /**
         * Returns an array of CollisionFlags that indicate if a Projectile can be positioned on a tile.
         *
         * @return The array of CollisionFlags.
         */
        public static CollisionFlag[] Projectiles()
        {
            return new CollisionFlag[] {
                CollisionFlag.Projectile_North_West,
                CollisionFlag.Projectile_North,
                CollisionFlag.Projectile_North_East,
                CollisionFlag.Projectile_West,
                CollisionFlag.Projectile_East,
                CollisionFlag.Projectile_South_West,
                CollisionFlag.Projectile_South,
                CollisionFlag.Projectile_South_East,
            };
        }

        public static short AsShort(this CollisionFlag collisionFlag)
        {
            return (short)(1 << collisionFlag.GetBit());
        }

        public static int GetBit(this CollisionFlag collisionFlag)
        {
            return (int)collisionFlag;
        }

        public static CollisionFlag[] ForType(EntityType entity)
        {
            return entity == EntityType.Projectile ? Projectiles() : Mobs();
        }
    }
}
