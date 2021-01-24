using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Extensions
{
    public static class EntityTypeExtensions
    {
        /// <summary>
        /// Determines whether this instance is mob.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        ///   <c>true</c> if the specified entity type is mob; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMob(this EntityType entityType)
        {
            return entityType == EntityType.Player || entityType == EntityType.Npc;
        }

        /// <summary>
        /// Returns whether or not this EntityType should be short-lived (i.e. not added to its regions local objects).
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        ///   <c>true</c> if the specified entity type is short-lived; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransient(this EntityType entityType)
        {
            return entityType == EntityType.Projectile;
        }
    }
}
