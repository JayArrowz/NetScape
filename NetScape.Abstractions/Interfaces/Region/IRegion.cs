using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using System.Collections.Generic;

namespace NetScape.Abstractions.Interfaces.Region
{
    public interface IRegion
    {
        /// <summary>
        /// The region size
        /// </summary>
        public static readonly int Size = 8;

        /// <summary>
        /// The default size of newly-created Lists, to reduce memory usage.
        /// </summary>
        public static readonly int Default_List_Size = 2;

        /// <summary>
        /// The viewable region radius
        /// </summary>
        public static readonly int Viewable_Region_Radius = 3;

        /// <summary>
        /// The width of the viewport of every Player, in tiles.
        /// </summary>
        public static readonly int Viewport_Width = Size * 13;

        RegionCoordinates Coordinates { get; }

        /// <summary>
        /// Adds a <paramref name="entity"/> to the Region. Note that this does not spawn the Entity, or do any other action other than
        /// register it to this Region.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void AddEntity(Entity entity);

        /// <summary>
        /// Adds a <see cref="Entity"/> to the Region. Note that this does not spawn the Entity, or do any other action other than
        /// register it to this Region.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="notify">if set to <c>true</c> [notify <see cref="IRegionListener"/>].</param>
        void AddEntity(Entity entity, bool notify);

        /// <summary>
        /// Adds a region listener to the region.
        /// </summary>
        /// <param name="listener">The listener.</param>
        void AddListener(IRegionListener listener);

        /// <summary>
        /// Checks if this Region contains the specified Entity.
        /// This method operates in constant time.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        ///   <c>true</c> if [this Region contains] [the specified entity]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(Entity entity);

        /// <summary>
        /// Returns whether or not the specified Position is inside this Region.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        ///   <c>true</c> if [Position] [inside region]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(Position position);

        /// <summary>
        /// Encodes the contents of this Region into a <see cref="HashSet{RegionUpdateMessage}"/>, to be sent to a client.
        /// </summary>
        /// <param name="height">The height level.</param>
        /// <returns>A set of region update messages</returns>
        HashSet<RegionUpdateMessage> Encode(int height);

        /// <summary>
        /// Gets a shallow copy of the HashSet of Entity objects at the specified Position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Entities</returns>
        HashSet<Entity> GetEntities(Position position);

        /// <summary>
        /// Gets an intermediate Set of Entities with the specified <paramref name="types"/> (s).
        /// </summary>
        /// <typeparam name="T">The entity object type</typeparam>
        /// <param name="types">The entity type.</param>
        /// <returns>A collection of <typeparam name="T"/></returns>
        HashSet<T> GetEntities<T>(params EntityType[] types) where T : Entity;

        HashSet<T> GetEntities<T>(Position position, params EntityType[] types) where T : Entity;

        /// <summary>
        /// Gets all the matrices in this region.
        /// </summary>
        /// <returns>The CollisionMatrix's</returns>
        CollisionMatrix[] GetMatrices();

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The CollisionMatrix</returns> 
        CollisionMatrix GetMatrix(int height);

        /// <summary>
        /// Gets the surrounding <see cref="RegionCoordinates"/> that are viewable from the specified <see cref="Position"/>.
        /// </summary>
        /// <returns>A set of RegionCoordinates</returns>
        HashSet<RegionCoordinates> GetSurrounding();

        /// <summary>
        /// Gets the Set of <see cref="RegionUpdateMessage"/>s that have occurred in the last pulse. This method can
        /// only be called <strong>once</strong> per pulse.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The Set of RegionUpdateMessages</returns>
        HashSet<RegionUpdateMessage> GetUpdates(int height);

        /// <summary>
        /// Notifies the <see cref="IRegionListener"/>s registered to this Region that an update has occurred.
        /// </summary>
        /// <param name="entity">The entity that was updated.</param>
        /// <param name="type">The update type that occured.</param>
        void NotifyListeners(Entity entity, EntityUpdateType type);

        /// <summary>
        /// Records the specified <see cref="IGroupableEntity"/> as being updated this pulse.
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="update">The update type.</param>
        void Record<T>(T entity, EntityUpdateType update) where T : Entity;

        /// <summary>
        /// Removes the entity from this region
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        void RemoveEntity(Entity entity);

        /// <summary>
        /// Returns whether or not an Entity of the specified {@link EntityType type} can traverse the tile at the specified
        /// coordinate pair.
        /// </summary>
        /// <param name="position">The position of the tile.</param>
        /// <param name="entity">The entity type <see cref="EntityType"/>.</param>
        /// <param name="direction">The <see cref="Direction"/>.</param>
        /// <returns><c>true</c> [if position] [is traversable] <c>false</c> [if not]</returns>
        bool Traversable(Position position, EntityType entity, Direction direction);
    }
}