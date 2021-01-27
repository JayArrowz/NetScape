using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Interfaces.Region.Collision
{
    /// <summary>
    /// Manages applying <see cref="ICollisionUpdate"/>s to the appropriate <see cref="Model.Region.Collision.CollisionMatrix"/>, and keeping
    /// a record of collision state(i.e., which tiles are bridged).
    /// </summary>
    public interface ICollisionManager
    {
        /// <summary>
        /// Apply a <see cref="ICollisionUpdate"/> to the game world.
        /// </summary>
        /// <param name="update">The update to apply.</param>
        void Apply(ICollisionUpdate update);

        /// <summary>
        /// Marks a tile as completely untraversable from all directions.
        /// </summary>
        /// <param name="position">The <see cref="Position"/> of the tile.</param>
        void Block(Position position);

        /// <summary>
        /// Applies the initial {@link CollisionUpdate} to the {@link CollisionMatrix}es for all objects and tiles loaded
        /// from the cache.
        /// </summary>
        /// <param name="rebuilding">if set to <c>true</c> [<see cref="Model.Region.Collision.CollisionMatrix"/>es are being rebuilt].</param>
        void Build(bool rebuilding);

        /// <summary>
        /// Marks a tile as part of a bridge.
        /// </summary>
        /// <param name="position">The <see cref="Position"/> of the tile.</param>
        void MarkBridged(Position position);

        /// <summary>
        /// Casts a ray into the world to check for impenetrable objects  from the given <paramref name="start"/> position to the
        /// <paramref name="end"/> position using Bresenham's line algorithm.
        /// </summary>
        /// <param name="start">The start position of the ray.</param>
        /// <param name="end">The end position of the ray.</param>
        /// <returns><c>true</c> if [an impentrable object was hit] else <c>false</c> [otherwise]</returns>
        bool Raycast(Position start, Position end);

        /// <summary>
        /// Checks if the given <see cref="EntityType"/> can traverse to the next tile from <paramref name="position"/> in the given
        /// <paramref name="direction"/>
        /// </summary>
        /// <param name="position">The current position of the entity.</param>
        /// <param name="type">The type of the entity.</param>
        /// <param name="direction">The direction the entity is travelling.</param>
        /// <returns><c>true</c> if [next tile is traversable] otherwise <c>false</c></returns>
        bool Traversable(Position position, EntityType type, Direction direction);
    }
}