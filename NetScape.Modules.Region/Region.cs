using Dawn;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using NetScape.Abstractions.Model.Region.Collision;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NetScape.Modules.Region
{

    public class UpdateRegionListener : IRegionListener
    {

        public void Execute(IRegion region, Entity entity, EntityUpdateType type)
        {
            var entityType = entity.EntityType;
            if (!entityType.IsMob() && entity is IGroupableEntity)
            {
                region.Record(entity, type);
            }
        }
    }

    /// <summary>
    /// An 8x8 area of the map.
    /// </summary>
    public class Region : IRegion
    {
        public RegionCoordinates Coordinates { get; }

        /// <summary>
        /// The Map of Positions to Entities in that Position.
        /// </summary>
        private readonly ConcurrentDictionary<Position, HashSet<Entity>> Entities = new();

        /// <summary>
        ///  A List of RegionListeners registered to this Region.
        /// </summary>     
        private readonly List<IRegionListener> Listeners = new();

        /// <summary>
        /// The CollisionMatrix.
        /// </summary>
        private readonly CollisionMatrix[] Matrices = CollisionMatrix.CreateMatrices(Position.HeightLevels, IRegion.Size, IRegion.Size);

        /// <summary>
        /// The List of Sets containing RegionUpdateMessages that specifically remove StaticGameObjects. The
        /// List is ordered based on the height level the RegionUpdateMessages concern.
        /// </summary>
        private readonly List<HashSet<RegionUpdateMessage>> RemovedObjects = new(Position.HeightLevels);

        /// <summary>
        /// The List of Sets containing RegionUpdateMessages. The List is ordered based on the height level the
        /// RegionUpdateMessages concern.This only contains the updates to this Region that have occurred in the last
        /// pulse.
        /// </summary>
        private readonly List<HashSet<RegionUpdateMessage>> Updates = new(Position.HeightLevels);

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="x">The x coordinate of the Region.</param>
        /// <param name="y">The y coordinate of the Region.</param>    
        public Region(int x, int y) : this(new RegionCoordinates(x, y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="coordinates">The RegionCoordinates.</param>
        public Region(RegionCoordinates coordinates)
        {
            Coordinates = coordinates;
            Listeners.Add(new UpdateRegionListener());

            for (int height = 0; height < Position.HeightLevels; height++)
            {
                RemovedObjects.Add(new());
                Updates.Add(new(IRegion.Default_List_Size));
            }
        }

        /// <summary>
        /// Adds a <see cref="Entity"/> to the Region. Note that this does not spawn the Entity, or do any other action other than
        /// register it to this Region.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="notify">if set to <c>true</c> [notify <see cref="IRegionListener"/>].</param>
        public void AddEntity(Entity entity, bool notify)
        {
            EntityType type = entity.EntityType;
            Position position = entity.Position;
            CheckPosition(position);

            if (!type.IsTransient())
            {
                HashSet<Entity> local = Entities.GetOrAdd(position, key => new HashSet<Entity>(IRegion.Default_List_Size));
                local.Add(entity);
            }

            if (notify)
            {
                NotifyListeners(entity, EntityUpdateType.Add);
            }
        }

        /// <summary>
        /// Adds a <paramref name="entity"/> to the Region. Note that this does not spawn the Entity, or do any other action other than
        /// register it to this Region.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void AddEntity(Entity entity)
        {
            AddEntity(entity, true);
        }

        public void AddListener(IRegionListener listener)
        {
            Listeners.Add(listener);
        }

        /// <summary>
        /// Checks if this Region contains the specified Entity.
        /// This method operates in constant time.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        ///   <c>true</c> if [this Region contains] [the specified entity]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Entity entity)
        {
            Position position = entity.Position;
            bool exists = Entities.TryGetValue(position, out var local);
            return local != null && local.Contains(entity);
        }

        /// <summary>
        /// Returns whether or not the specified Position is inside this Region.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        ///   <c>true</c> if [Position] [inside region]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Position position)
        {
            return Coordinates.Equals(position.RegionCoordinates);
        }

        /// <summary>
        /// Encodes the contents of this Region into a <see cref="HashSet{RegionUpdateMessage}"/>, to be sent to a client.
        /// </summary>
        /// <param name="height">The height level.</param>
        /// <returns>A set of region update messages</returns>
        public HashSet<RegionUpdateMessage> Encode(int height)
        {
            HashSet<RegionUpdateMessage> additions = Entities.Values
                .SelectMany(t => t)
                .Where(entity => entity is DynamicGameObject && entity.Position.Height == height)
                .Select(entity => ((IGroupableEntity)entity).ToUpdateOperation(this, EntityUpdateType.Add).ToMessage())
                .ToHashSet();
            HashSet<RegionUpdateMessage> allUpdates = new();

            allUpdates.UnionWith(additions);
            allUpdates.UnionWith(Updates[height]);
            allUpdates.UnionWith(RemovedObjects[height]);
            return allUpdates;
        }

        /// <summary>
        /// Gets an intermediate Set of Entities with the specified <paramref name="types"/> (s).
        /// </summary>
        /// <typeparam name="T">The entity object type</typeparam>
        /// <param name="types">The entity type.</param>
        /// <returns>A collection of <typeparam name="T"/></returns>
        public HashSet<T> GetEntities<T>(params EntityType[] types) where T : Entity
        {
            return Entities.Values.SelectMany(t => t)
                .Where(entity => entity is T && types.Contains(entity.EntityType))
                .Cast<T>()
                .ToHashSet();
        }


        /// <summary>
        /// Gets a shallow copy of the HashSet of Entity objects at the specified Position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Entities</returns>
        public HashSet<Entity> GetEntities(Position position)
        {
            Entities.TryGetValue(position, out var set);
            return (set == null) ? new() : set.ToHashSet();
        }


        public HashSet<T> GetEntities<T>(Position position, params EntityType[] types) where T : Entity
        {
            Entities.TryGetValue(position, out var local);
            if (local == null)
            {
                return new();
            }

            HashSet<EntityType> set = types.ToHashSet();
            HashSet<T> filtered = local.Where(entity => set.Contains(entity.EntityType))
                .Cast<T>()
                .ToHashSet();
            return filtered;
        }

        /// <summary>
        /// Gets the surrounding <see cref="RegionCoordinates"/> that are viewable from the specified <see cref="Position"/>.
        /// </summary>
        /// <returns>A set of RegionCoordinates</returns>
        public HashSet<RegionCoordinates> GetSurrounding()
        {
            int localX = Coordinates.X, localY = Coordinates.Y;
            int maxX = localX + IRegion.Viewable_Region_Radius, maxY = localY + IRegion.Viewable_Region_Radius;

            HashSet<RegionCoordinates> viewable = new();
            for (int x = localX - IRegion.Viewable_Region_Radius; x < maxX; x++)
            {
                for (int y = localY - IRegion.Viewable_Region_Radius; y < maxY; y++)
                {
                    viewable.Add(new RegionCoordinates(x, y));
                }
            }

            return viewable;
        }

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The CollisionMatrix</returns> 
        public CollisionMatrix GetMatrix(int height)
        {
            return Matrices[height];
        }

        /// <summary>
        /// Gets all the matrices in this region.
        /// </summary>
        /// <returns>The CollisionMatrix's</returns>
        public CollisionMatrix[] GetMatrices()
        {
            return Matrices;
        }

        /// <summary>
        /// Gets the Set of <see cref="RegionUpdateMessage"/>s that have occurred in the last pulse. This method can
        /// only be called <strong>once</strong> per pulse.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The Set of RegionUpdateMessages</returns>
        public HashSet<RegionUpdateMessage> GetUpdates(int height)
        {
            HashSet<RegionUpdateMessage> updates = Updates[height];
            var copy = updates.ToHashSet();
            updates.Clear();
            return copy;
        }

        /// <summary>
        /// Notifies the <see cref="IRegionListener"/>s registered to this Region that an update has occurred.
        /// </summary>
        /// <param name="entity">The entity that was updated.</param>
        /// <param name="type">The update type that occured.</param>
        public void NotifyListeners(Entity entity, EntityUpdateType type)
        {
            Listeners.ForEach(listener => listener.Execute(this, entity, type));
        }

        /// <summary>
        /// Removes the entity from this region
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">
        /// Tried to remove a transient Entity
        /// </exception>
        public void RemoveEntity(Entity entity)
        {
            EntityType type = entity.EntityType;
            if (type.IsTransient())
            {
                throw new NotSupportedException("Tried to remove a transient Entity (" + entity + ") from " +
                    "(" + this + ").");
            }

            Position position = entity.Position;
            CheckPosition(position);

            Entities.TryGetValue(position, out var local);

            if (local == null || !local.Remove(entity))
            {
                throw new NotSupportedException("Entity (" + entity + ") belongs in (" + this + ") but does not exist.");
            }

            NotifyListeners(entity, EntityUpdateType.Remove);
        }

        /// <summary>
        /// Returns whether or not an Entity of the specified {@link EntityType type} can traverse the tile at the specified
        /// coordinate pair.
        /// </summary>
        /// <param name="position">The position of the tile.</param>
        /// <param name="entity">The entity type <see cref="EntityType"/>.</param>
        /// <param name="direction">The <see cref="Direction"/>.</param>
        /// <returns><c>true</c> [if position] [is traversable] <c>false</c> [if not]</returns>
        public bool Traversable(Position position, EntityType entity, Direction direction)
        {
            CollisionMatrix matrix = Matrices[position.Height];
            int x = position.X, y = position.Y;

            return !matrix.Untraversable(x % IRegion.Size, y % IRegion.Size, entity, direction);
        }

        /// <summary>
        /// Checks that the specified <see cref="Position"/> is included in this Region.
        /// </summary>
        /// <param name="position">The position.</param>
        private void CheckPosition(Position position)
        {
            Guard.Argument(Coordinates).Equal(RegionCoordinates.FromPosition(position));
        }

        /// <summary>
        /// Records the specified <see cref="IGroupableEntity"/> as being updated this pulse.
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="update">The update type.</param>
        public void Record<T>(T entity, EntityUpdateType update) where T : Entity
        {
            IRegionUpdateOperation operation = ((IGroupableEntity)entity).ToUpdateOperation(this, update);
            RegionUpdateMessage message = operation.ToMessage(), inverse = operation.Inverse();

            int height = entity.Position.Height;
            HashSet<RegionUpdateMessage> updates = Updates[height];

            EntityType type = entity.EntityType;

            if (type == EntityType.Static_Object)
            { // TODO set/clear collision matrix values
                if (update == EntityUpdateType.Remove)
                {
                    RemovedObjects[height].Add(message);
                }
                else
                { // TODO should this really be possible?
                    RemovedObjects[height].Remove(inverse);
                }
            }

            if (update == EntityUpdateType.Remove && !type.IsTransient())
            {
                updates.Remove(inverse);
            }

            updates.Add(message);
        }
    }
}
