namespace NetScape.Abstractions.Model.Region.Collision
{
    public enum CollisionUpdateType
    {
        /// <summary>
        /// Indicates that a <see cref="Interfaces.Region.Collision.ICollisionUpdate"/> will be adding new flags to collision matrices
        /// </summary>
        Adding,

        /// <summary>
        /// Indicates that a <see cref="Interfaces.Region.Collision.ICollisionUpdate"/> will be clearing existing flags from collision matrices.
        /// </summary>
        Removing
    }
}
