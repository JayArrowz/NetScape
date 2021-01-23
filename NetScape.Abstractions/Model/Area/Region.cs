using Dawn;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Interfaces.Area;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.Abstractions.Model.Area
{

    public class UpdateRegionListener : IRegionListener
    {

        public void Execute(Region region, Entity entity, EntityUpdateType type)
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
    public class Region
    {
        public static readonly int Size = 8;

        /**
         * The default size of newly-created Lists, to reduce memory usage.
         */
        private static readonly int DEFAULT_LIST_SIZE = 2;
        public static readonly int VIEWABLE_REGION_RADIUS = 3;

        /**
         * The width of the viewport of every Player, in tiles.
         */
        public static readonly int Viewport_Width = Size * 13;

        public RegionCoordinates Coordinates { get; }

        /**
         * The Map of Positions to Entities in that Position.
         */
        private readonly ConcurrentDictionary<Position, HashSet<Entity>> Entities = new();

        /**
         * A List of RegionListeners registered to this Region.
         */
        private readonly List<IRegionListener> Listeners = new();

        /**
         * The CollisionMatrix.
         */
        private readonly CollisionMatrix[] Matrices = CollisionMatrix.CreateMatrices(Position.HeightLevels, Size, Size);

        /**
         * The List of Sets containing RegionUpdateMessages that specifically remove StaticGameObjects. The
         * List is ordered based on the height level the RegionUpdateMessages concern.
         */
        private readonly List<HashSet<RegionUpdateMessage>> RemovedObjects = new(Position.HeightLevels);

        /**
         * The List of Sets containing RegionUpdateMessages. The List is ordered based on the height level the
         * RegionUpdateMessages concern. This only contains the updates to this Region that have occurred in the last
         * pulse.
         */
        private readonly List<HashSet<RegionUpdateMessage>> Updates = new(Position.HeightLevels);

        /**
		 * Creates a new Region.
		 *
		 * @param x The x coordinate of the Region.
		 * @param y The y coordinate of the Region.
		 */
        public Region(int x, int y) : this(new RegionCoordinates(x, y))
        {
        }

        /**
		 * Creates a new Region with the specified {@link RegionCoordinates}.
		 *
		 * @param coordinates The RegionCoordinates.
		 */
        public Region(RegionCoordinates coordinates)
        {
            Coordinates = coordinates;
            Listeners.Add(new UpdateRegionListener());

            for (int height = 0; height < Position.HeightLevels; height++)
            {
                RemovedObjects.Add(new());
                Updates.Add(new(DEFAULT_LIST_SIZE));
            }
        }

        /**
		 * Adds a {@link Entity} to the Region. Note that this does not spawn the Entity, or do any other action other than
		 * register it to this Region.
		 *
		 * @param entity The Entity.
		 * @param notify Whether or not the {@link RegionListener}s for this Region should be notified.
		 * @throws IllegalArgumentException If the Entity does not belong in this Region.
		 */
        public void addEntity(Entity entity, bool notify)
        {
            EntityType type = entity.EntityType;
            Position position = entity.Position;
            checkPosition(position);

            if (!type.IsTransient())
            {
                HashSet<Entity> local = Entities.GetOrAdd(position, key => new HashSet<Entity>(DEFAULT_LIST_SIZE));
                local.Add(entity);
            }

            if (notify)
            {
                NotifyListeners(entity, EntityUpdateType.Add);
            }
        }

        /**
		 * Adds a {@link Entity} to the Region. Note that this does not spawn the Entity, or do any other action other than
		 * register it to this Region.
		 *
		 * By default, this method notifies RegionListeners for this region of the addition.
		 *
		 * @param entity The Entity.
		 * @throws IllegalArgumentException If the Entity does not belong in this Region.
		 */
        public void addEntity(Entity entity)
        {
            addEntity(entity, true);
        }

        public void addListener(IRegionListener listener)
        {
            Listeners.Add(listener);
        }

        /**
		 * Checks if this Region contains the specified Entity.
		 *
		 * This method operates in constant time.
		 *
		 * @param entity The Entity.
		 * @return {@code true} if this Region contains the Entity, otherwise {@code false}.
		 */
        public bool contains(Entity entity)
        {
            Position position = entity.Position;
            bool exists = Entities.TryGetValue(position, out var local);
            return local != null && local.Contains(entity);
        }

        /**
		 * Returns whether or not the specified {@link Position} is inside this Region.
		 *
		 * @param position The Position.
		 * @return {@code true} iff the specified Position is inside this Region.
		 */
        public bool contains(Position position)
        {
            return Coordinates.Equals(position.RegionCoordinates);
        }

        /**
		 * Encodes the contents of this Region into a {@link Set} of {@link RegionUpdateMessage}s, to be sent to a client.
		 *
		 * @return The Set of RegionUpdateMessages.
		 */
        public HashSet<RegionUpdateMessage> encode(int height)
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

        /**
		 * Gets an intermediate {@link Stream} from the {@link Set} of
		 * {@link Entity}s with the specified {@link EntityType} (s). Type will be
		 * inferred from the call, so ensure that the Entity type and the reference
		 * correspond, or this method will fail at runtime.
		 *
		 * @param types The {@link EntityType}s.
		 * @return The Stream of Entity objects.
		 */
        public HashSet<T> GetEntities<T>(params EntityType[] types) where T : Entity
        {
            return Entities.Values.SelectMany(t => t)
                .Where(entity => entity is T && types.Contains(entity.EntityType))
                .Cast<T>()
                .ToHashSet();
        }


        /**
		 * Gets a shallow copy of the {@link Set} of {@link Entity} objects at the specified {@link Position}. The returned
		 * type will be immutable.
		 *
		 * @param position The Position containing the entities.
		 * @return The Set. Will be immutable.
		 */
        public HashSet<Entity> getEntities(Position position)
        {
            Entities.TryGetValue(position, out var set);
            return (set == null) ? new() : set.ToHashSet();
        }

        /**
		 * Gets a shallow copy of the {@link Set} of {@link Entity}s with the specified {@link EntityType}(s). The returned
		 * type will be immutable. Type will be inferred from the call, so ensure that the Entity type and the reference
		 * correspond, or this method will fail at runtime.
		 *
		 * @param position The {@link Position} containing the entities.
		 * @param types The {@link EntityType}s.
		 * @return The Set of Entity objects.
		 */
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

        /**
		 * Gets the {@link Set} of {@link RegionCoordinates} of Regions that are viewable from the specified
		 * {@link Position}.
		 *
		 * @return The Set of RegionCoordinates.
		 */
        public HashSet<RegionCoordinates> GetSurrounding()
        {
            int localX = Coordinates.X, localY = Coordinates.Y;
            int maxX = localX + VIEWABLE_REGION_RADIUS, maxY = localY + VIEWABLE_REGION_RADIUS;

            HashSet<RegionCoordinates> viewable = new();
            for (int x = localX - VIEWABLE_REGION_RADIUS; x < maxX; x++)
            {
                for (int y = localY - VIEWABLE_REGION_RADIUS; y < maxY; y++)
                {
                    viewable.Add(new RegionCoordinates(x, y));
                }
            }

            return viewable;
        }

        /**
		 * Gets the {@link CollisionMatrix} at the specified height level.
		 *
		 * @param height The height level.
		 * @return The CollisionMatrix.
		 */
        public CollisionMatrix GetMatrix(int height)
        {
            return Matrices[height];
        }

        /**
		 * Gets all of the {@link CollisionMatrix} objects in this {@code Region}.
		 *
		 * @return The collision matrices of this region.
		 */
        public CollisionMatrix[] GetMatrices()
        {
            return Matrices;
        }

        /**
		 * Gets the {@link Set} of {@link RegionUpdateMessage}s that have occurred in the last pulse. This method can
		 * only be called <strong>once</strong> per pulse.
		 *
		 * @param height The height level to get the RegionUpdateMessages for.
		 * @return The Set of RegionUpdateMessages.
		 */
        public HashSet<RegionUpdateMessage> GetUpdates(int height)
        {
            HashSet<RegionUpdateMessage> updates = Updates[height];
            var copy = updates.ToHashSet();
            updates.Clear();
            return copy;
        }

        /**
		 * Notifies the {@link RegionListener}s registered to this Region that an update has occurred.
		 *
		 * @param entity The {@link Entity} that was updated.
		 * @param type The {@link EntityUpdateType} that occurred.
		 */
        public void NotifyListeners(Entity entity, EntityUpdateType type)
        {
            Listeners.ForEach(listener => listener.Execute(this, entity, type));
        }

        /**
		 * Removes an {@link Entity} from this Region.
		 *
		 * @param entity The Entity.
		 * @throws IllegalArgumentException If the Entity does not belong in this Region, or if it was never added.
		 */
        public void removeEntity(Entity entity)
        {
            EntityType type = entity.EntityType;
            if (type.IsTransient())
            {
                throw new NotSupportedException("Tried to remove a transient Entity (" + entity + ") from " +
                    "(" + this + ").");
            }

            Position position = entity.Position;
            checkPosition(position);

            Entities.TryGetValue(position, out var local);

            if (local == null || !local.Remove(entity))
            {
                throw new NotSupportedException("Entity (" + entity + ") belongs in (" + this + ") but does not exist.");
            }

            NotifyListeners(entity, EntityUpdateType.Remove);
        }

        /**
		 * Returns whether or not an Entity of the specified {@link EntityType type} can traverse the tile at the specified
		 * coordinate pair.
		 *
		 * @param position The {@link Position} of the tile.
		 * @param entity The {@link EntityType}.
		 * @param direction The {@link Direction} the Entity is approaching from.
		 * @return {@code true} if the tile at the specified coordinate pair is traversable, {@code false} if not.
		 */
        public bool traversable(Position position, EntityType entity, Direction direction)
        {
            CollisionMatrix matrix = Matrices[position.Height];
            int x = position.X, y = position.Y;

            return !matrix.Untraversable(x % Size, y % Size, entity, direction);
        }

        /**
		 * Checks that the specified {@link Position} is included in this Region.
		 *
		 * @param position The position.
		 * @throws IllegalArgumentException If the specified position is not included in this Region.
		 */
        private void checkPosition(Position position)
        {
            Guard.Argument(Coordinates).Equal(RegionCoordinates.FromPosition(position));
        }

        /**
		 * Records the specified {@link GroupableEntity} as being updated this pulse.
		 *
		 * @param entity The GroupableEntity.
		 * @param update The {@link EntityUpdateType}.
		 * @throws UnsupportedOperationException If the specified Entity cannot be operated on in this manner.
		 */
        public void Record<T>(T entity, EntityUpdateType update) where T : Entity
        {
            UpdateOperation operation = ((IGroupableEntity)entity).ToUpdateOperation(this, update);
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
