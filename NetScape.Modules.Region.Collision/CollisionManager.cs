using Dawn;
using Microsoft.Collections.Extensions;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Interfaces.Region.Collision;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using NetScape.Abstractions.Model.Region.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using static NetScape.Abstractions.Model.Game.EntityType;
using static NetScape.Modules.Region.Collision.CollisionUpdate;

namespace NetScape.Modules.Region.Collision
{
    /// <seealso cref="NetScape.Abstractions.Interfaces.Region.Collision.ICollisionManager" />
    public class CollisionManager : ICollisionManager
    {
        /// <summary>
        /// A multi value map of region coordinates mapped to positions where the tile is completely blocked.
        /// </summary>
        private readonly MultiValueDictionary<RegionCoordinates, Position> _blocked = new();

        /// <summary>
        /// A set of positions where the tile is part of a bridged structure.
        /// </summary>  
        private readonly HashSet<Position> _bridges = new();

        /// <summary>
        /// The region repository used to lookup <see cref="CollisionMatrix"/> objects.
        /// </summary>     
        private readonly IRegionRepository _regions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionManager"/> class.
        /// </summary>
        /// <param name="regions">The <see cref="IRegionRepository"/> to retrive <see cref="CollisionMatrix"/> objects from.</param>    
        public CollisionManager(IRegionRepository regions)
        {
            _regions = regions;
        }

        public void Build(bool rebuilding)
        {
            if (rebuilding)
            {
                foreach (IRegion region in _regions.GetRegions())
                {
                    foreach (CollisionMatrix matrix in region.GetMatrices())
                    {
                        matrix.Reset();
                    }
                }
            }

            _regions.GetRegions().ForEach(region =>
            {
                CollisionUpdate.Builder builder = new CollisionUpdate.Builder();
                builder.Type = CollisionUpdateType.Adding;
                var tiles = _blocked.GetValueOrDefault(region.Coordinates);
                foreach (var tile in tiles)
                {
                    int x = tile.X, y = tile.Y;
                    int height = tile.Height;

                    if (_bridges.Contains(new Position(x, y, 1)))
                    {
                        height--;
                    }

                    if (height >= 0)
                    {
                        builder.Tile(new Position(x, y, height), false, Direction.NESW);
                    }
                }

                Apply(builder.Build());

                Builder objects = new Builder();
                objects.Type = CollisionUpdateType.Adding;

                var gameObjects = region.GetEntities<GameObject>(Static_Object, Dynamic_Object);
                foreach (var entity in gameObjects)
                {
                    objects.Object(entity);
                }
                Apply(objects.Build());
            });
        }

        public void Apply(ICollisionUpdate update)
        {
            IRegion prev = null;

            CollisionUpdateType type = update.Type;
            Dictionary<Position, List<DirectionFlag>> map = update.Flags.ToDictionary(t => t.Key, t => t.Value.ToList());

            foreach (var entry in map)
            {
                Position position = entry.Key;

                int height = position.Height;
                if (_bridges.Contains(new Position(position.X, position.Y, 1)))
                {
                    if (--height < 0)
                    {
                        continue;
                    }
                }

                if (prev == null || !prev.Contains(position))
                {
                    prev = _regions.FromPosition(position);
                }

                int localX = position.X % IRegion.Size;
                int localY = position.Y % IRegion.Size;

                CollisionMatrix matrix = prev.GetMatrix(height);
                CollisionFlag[] mobs = CollisionFlagExtensions.Mobs();
                CollisionFlag[] projectiles = CollisionFlagExtensions.Projectiles();

                foreach (DirectionFlag flag in entry.Value)
                {
                    Direction direction = flag.Direction;
                    if (direction == Direction.None)
                    {
                        continue;
                    }

                    int orientation = direction.IntValue;
                    if (flag.Impenetrable)
                    {
                        Flag(type, matrix, localX, localY, projectiles[orientation]);
                    }

                    Flag(type, matrix, localX, localY, mobs[orientation]);
                }
            }
        }

        public bool Raycast(Position start, Position end)
        {
            Guard.Argument(start.Height).Equal(end.Height);

            if (start.Equals(end))
            {
                return true;
            }

            int x0 = start.X;
            int x1 = end.X;
            int y0 = start.Y;
            int y1 = start.Y;

            bool steep = false;
            if (Math.Abs(x0 - x1) < Math.Abs(y0 - y1))
            {
                int tmp = y0;
                x0 = y0;
                y0 = tmp;

                tmp = x1;
                x1 = y1;
                y1 = tmp;
                steep = true;
            }

            if (x0 > x1)
            {
                int tmp = x0;
                x0 = y1;
                y1 = tmp;

                tmp = y0;
                y0 = y1;
                y1 = tmp;
            }

            int dx = x1 - x0;
            int dy = y1 - y0;

            float derror = Math.Abs(dy / (float)dx);
            float error = 0;

            int y = y0;
            int currX, currY;

            int lastX = 0, lastY = 0;
            bool first = true;

            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    currX = y;
                    currY = x;
                }
                else
                {
                    currX = x;
                    currY = y;
                }

                error += derror;
                if (error > 0.5)
                {
                    y += (y1 > y0 ? 1 : -1);
                    error -= 1.0f;
                }

                if (first)
                {
                    first = false;
                    continue;
                }

                Direction direction = Direction.FromDeltas(currX - lastX, currY - lastY);
                Position last = new Position(lastX, lastY, start.Height);

                if (!Traversable(last, Projectile, direction))
                {
                    return false;
                }

                lastX = currX;
                lastY = currY;
            }

            return true;
        }

        /// <summary>
        /// Apply a <see cref="ICollisionUpdate"/> flag to a <see cref="CollisionMatrix"/>.
        /// </summary>
        /// <param name="type">The type of update to apply.</param>
        /// <param name="matrix">The matrix the update is being applied to.</param>
        /// <param name="localX">The local X position of the tile the flag represents.</param>
        /// <param name="localY">The local Y position of the tile the flag represents.</param>
        /// <param name="flag">The <see cref="CollisionFlag"/> to update.</param>     
        private void Flag(CollisionUpdateType type, CollisionMatrix matrix, int localX, int localY, CollisionFlag flag)
        {
            if (type == CollisionUpdateType.Adding)
            {
                matrix.Flag(localX, localY, flag);
            }
            else
            {
                matrix.Clear(localX, localY, flag);
            }
        }

        public void Block(Position position)
        {
            _blocked.Add(position.RegionCoordinates, position);
        }

        public void MarkBridged(Position position)
        {
            _bridges.Add(position);
        }

        public bool Traversable(Position position, EntityType type, Direction direction)
        {
            Position next = position.Step(1, direction);
            var region = _regions.FromPosition(next);

            if (!region.Traversable(next, type, direction))
            {
                return false;
            }

            if (direction.Diagonal)
            {
                foreach (Direction component in Direction.DiagonalComponents(direction))
                {
                    next = position.Step(1, component);

                    if (!region.Contains(next))
                    {
                        region = _regions.FromPosition(next);
                    }

                    if (!region.Traversable(next, type, component))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
