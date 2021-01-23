using NetScape.Abstractions.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Extensions
{
    public static class EntityTypeExtensions
    {
        /**
         * Returns whether or not this EntityType is for a Mob.
         *
         * @return {@code true} if this EntityType is for a Mob, otherwise {@code false}.
         */
        public static bool IsMob(this EntityType entityType)
        {
            return entityType == EntityType.Player || entityType == EntityType.Npc;
        }

        /**
         * Returns whether or not this EntityType should be short-lived (i.e. not added to its regions local objects).
         *
         * @return {@code true} if this EntityType is short-lived.
         */
        public static bool IsTransient(this EntityType entityType)
        {
            return entityType == EntityType.Projectile;
        }
    }
}
