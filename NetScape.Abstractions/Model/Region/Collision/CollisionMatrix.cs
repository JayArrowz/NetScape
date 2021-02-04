using Dawn;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Model.Game;
using System;
using System.Linq;

namespace NetScape.Abstractions.Model.Region.Collision
{
    public class CollisionMatrix
    {
        /// <summary>
        /// Indicates that all types of traversal are allowed.
        /// </summary>
        private static readonly short All_Allowed = 0;

        /// <summary>
        /// Indicates that no types of traversal are allowed.
        /// </summary>
        private static readonly short All_Blocked = -1;

        /// <summary>
        /// Indicates that projectiles may traverse this tile, but mobs may not.
        /// </summary>
        private static readonly short All_Mobs_Blocked = -256;

        /// <summary>
        /// Creates an array of CollisionMatrix objects, all of the specified width and length.
        /// </summary>
        /// <param name="count">The length of the array to create.</param>
        /// <param name="width">The width of each CollisionMatrix.</param>
        /// <param name="length">The length of each CollisionMatrix.</param>
        /// <returns>The array of CollisionMatrix objects</returns>
        public static CollisionMatrix[] CreateMatrices(int count, int width, int length)
        {
            return Enumerable.Range(0, count).Select(t => new CollisionMatrix(width, length)).ToArray();
        }

        /// <summary>
        /// The length of the matrix.
        /// </summary>
        private readonly int _length;

        /// <summary>
        /// The collision matrix, as a <c>short</c> array.
        /// </summary>
        private readonly short[] _matrix;

        /// <summary>
        /// The width of the matrix.
        /// </summary>
        private readonly int _width;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionMatrix"/> class.
        /// </summary>
        /// <param name="width">The width of the matrix..</param>
        /// <param name="length">The length of the matrix.</param>
        public CollisionMatrix(int width, int length)
        {
            _width = width;
            _length = length;
            _matrix = new short[width * length];
        }

        /// <summary>
        /// Returns whether or not <strong>all</strong> of the specified <see cref="CollisionFlag"/>s are set for the specified
		/// coordinate pair.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="flags"> The CollisionFlags.</param>
        /// <returns>if true if all the CollisionFlags are set</returns>
        public bool All(int x, int y, params CollisionFlag[] flags)
        {
            foreach (CollisionFlag flag in flags)
            {
                if (!Flagged(x, y, flag))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether or not <strong>any</strong> of the specified <see cref="CollisionFlag"/>s are set for the specified
        /// coordinate pair.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="flags">The CollisionFlags.</param>
        /// <returns>true if any of the CollisionFlags are set.</returns>
        public bool Any(int x, int y, params CollisionFlag[] flags)
        {
            foreach (CollisionFlag flag in flags)
            {
                if (Flagged(x, y, flag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Completely blocks the tile at the specified coordinate pair, while optionally allowing projectiles
		/// to pass through.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="impenetrable">if set to <c>true</c> [impenetrable].</param>
        public void Block(int x, int y, bool impenetrable)
        {
            Set(x, y, impenetrable ? All_Blocked : All_Mobs_Blocked);
        }

        /// <summary>
        /// Completely blocks the tile at the specified coordinate pair.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void Block(int x, int y)
        {
            Block(x, y, true);
        }

        /// <summary>
        /// Clears (i.e. sets to <c>false</c>) the value of the specified <see cref="CollisionFlag"/> for the specified
		/// coordinate pair.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="flag">The CollisionFlag.</param>
        public void Clear(int x, int y, CollisionFlag flag)
        {
            Set(x, y, (short)(_matrix[IndexOf(x, y)] & ~flag.AsShort()));
        }

        /// <summary>
        /// Adds an additional <seealso cref="CollisionFlag"/> for the specified coordinate pair.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="flag">The CollisionFlag.</param>
        public void Flag(int x, int y, CollisionFlag flag)
        {
            _matrix[IndexOf(x, y)] |= flag.AsShort();
        }

        /// <summary>
        /// Returns whether or not the specified <see cref="CollisionFlag"/> is set for the specified coordinate pair.
        /// </summary>
        /// <param name="x">The x coord.</param>
        /// <param name="y">The y coord.</param>
        /// <param name="flag">The CollisionFlag.</param>
        /// <returns>true if the CollisionFlag is set</returns>
        public bool Flagged(int x, int y, CollisionFlag flag)
        {
            return (Get(x, y) & flag.AsShort()) != 0;
        }

        /// <summary>
        /// Gets the value of the specified tile.
        /// </summary>
        /// <param name="x">The x coordinate of the tile.</param>
        /// <param name="y">The y coordinate of the tile..</param>
        /// <returns>The matrix value</returns>
        public int Get(int x, int y)
        {
            return _matrix[IndexOf(x, y)] & 0xFFFF;
        }

        /// <summary>
        /// Resets the cell of the specified coordinate pair.
        /// </summary>
        /// <param name="x">The x coord.</param>
        /// <param name="y">The y coord.</param>
        public void Reset(int x, int y)
        {
            Set(x, y, All_Allowed);
        }

        /// <summary>
        /// Resets all cells in this matrix
        /// </summary>
        public void Reset()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _width; y++)
                {
                    Reset(x, y);
                }
            }
        }

        /// <summary>
        /// Sets (i.e. sets to <code>true</code>) the value of the specified <see cref="CollisionFlag"/> for the specified coordinate
		/// pair.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="flag">The flag.</param>
        public void Set(int x, int y, CollisionFlag flag)
        {
            Set(x, y, flag.AsShort());
        }

        /// <summary>
        /// Returns whether or not an Entity of the specified <see cref="EntityType"/> cannot traverse the tile at the
		/// specified coordinate pair.
        /// </summary>
        /// <param name="x">The x coord.</param>
        /// <param name="y">The y coord.</param>
        /// <param name="entity">The entity type.</param>
        /// <param name="direction">The direction the entity is approaching from.</param>
        /// <returns><code>true</code> iff the tile at the specified coordinate pair is not traversable</returns>
        /// <exception cref="ArgumentException">Unrecognised direction {direction}</exception>
        public bool Untraversable(int x, int y, EntityType entity, Direction direction)
        {
            CollisionFlag[] flags = CollisionFlagExtensions.ForType(entity);
            int northwest = 0, north = 1, northeast = 2, west = 3, east = 4, southwest = 5, south = 6, southeast = 7;

            switch (direction.IntValue)
            {
                case 0:
                    return Flagged(x, y, flags[southeast]) || Flagged(x, y, flags[south]) || Flagged(x, y, flags[east]);
                case 1:
                    return Flagged(x, y, flags[south]);
                case 2:
                    return Flagged(x, y, flags[southwest]) || Flagged(x, y, flags[south]) || Flagged(x, y, flags[west]);
                case 4:
                    return Flagged(x, y, flags[west]);
                case 7:
                    return Flagged(x, y, flags[northwest]) || Flagged(x, y, flags[north]) || Flagged(x, y, flags[west]);
                case 6:
                    return Flagged(x, y, flags[north]);
                case 5:
                    return Flagged(x, y, flags[northeast]) || Flagged(x, y, flags[north]) || Flagged(x, y, flags[east]);
                case 3:
                    return Flagged(x, y, flags[east]);
                default:
                    throw new ArgumentException($"Unrecognised direction {direction}");
            }
        }

        /// <summary>
        /// Gets the index in the matrix for the specified coordinate pair.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>index in the matrix</returns>
        private int IndexOf(int x, int y)
        {
            Guard.Argument(x).GreaterThan(-1).LessThan(_width + 1);
            Guard.Argument(y).GreaterThan(-1).LessThan(_length + 1);
            return y * _width + x;
        }

        /// <summary>
        /// Sets the appropriate index for the specified coordinate pair to the specified value.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="value">The value.</param>
        private void Set(int x, int y, short value)
        {
            _matrix[IndexOf(x, y)] = value;
        }
    }
}
