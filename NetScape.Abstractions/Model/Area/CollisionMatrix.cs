using Dawn;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Model.Game;
using System;
using System.Linq;

namespace NetScape.Abstractions.Model.Area
{
    public class CollisionMatrix
    {

        /**
		 * Indicates that all types of traversal are allowed.
		 */
        private static readonly short All_Allowed = 0;

        /**
		 * Indicates that no types of traversal are allowed.
		 */
        private static readonly short All_Blocked = -1;

        /**
		 * Indicates that projectiles may traverse this tile, but mobs may not.
		 */
        private static readonly short All_Mobs_Blocked = -256;

        /**
		 * Creates an array of CollisionMatrix objects, all of the specified width and length.
		 *
		 * @param count The length of the array to create.
		 * @param width The width of each CollisionMatrix.
		 * @param length The length of each CollisionMatrix.
		 * @return The array of CollisionMatrix objects.
		 */
        public static CollisionMatrix[] CreateMatrices(int count, int width, int length)
        {
            return Enumerable.Range(0, count).Select(t => new CollisionMatrix(width, length)).ToArray();
        }

        /**
		 * The length of the matrix.
		 */
        private readonly int length;

        /**
		 * The collision matrix, as a {@code short} array.
		 */
        private readonly short[] matrix;

        /**
		 * The width of the matrix.
		 */
        private readonly int width;

        /**
		 * Creates the CollisionMatrix.
		 *
		 * @param width The width of the matrix.
		 * @param length The length of the matrix.
		 */
        public CollisionMatrix(int width, int length)
        {
            this.width = width;
            this.length = length;
            matrix = new short[width * length];
        }

        /**
		 * Returns whether or not <strong>all</strong> of the specified {@link CollisionFlag}s are set for the specified
		 * coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param flags The CollisionFlags.
		 * @return {@code true} iff all of the CollisionFlags are set.
		 */
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

        /**
		 * Returns whether or not <strong>any</strong> of the specified {@link CollisionFlag}s are set for the specified
		 * coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param flags The CollisionFlags.
		 * @return {@code true} iff any of the CollisionFlags are set.
		 */
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

        /**
		 * Completely blocks the tile at the specified coordinate pair, while optionally allowing projectiles
		 * to pass through.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param impenetrable If projectiles should be permitted to traverse this tile.
		 */
        public void Block(int x, int y, bool impenetrable)
        {
            Set(x, y, impenetrable ? All_Blocked : All_Mobs_Blocked);
        }

        /**
		 * Completely blocks the tile at the specified coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 */
        public void Block(int x, int y)
        {
            Block(x, y, true);
        }

        /**
		 * Clears (i.e. sets to {@code false}) the value of the specified {@link CollisionFlag} for the specified
		 * coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param flag The CollisionFlag.
		 */
        public void Clear(int x, int y, CollisionFlag flag)
        {
            Set(x, y, (short)(matrix[IndexOf(x, y)] & ~flag.AsShort()));
        }

        /**
		 * Adds an additional {@link CollisionFlag} for the specified coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param flag The CollisionFlag.
		 */
        public void Flag(int x, int y, CollisionFlag flag)
        {
            matrix[IndexOf(x, y)] |= flag.AsShort();
        }

        /**
		 * Returns whether or not the specified {@link CollisionFlag} is set for the specified coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param flag The CollisionFlag.
		 * @return {@code true} iff the CollisionFlag is set.
		 */
        public bool Flagged(int x, int y, CollisionFlag flag)
        {
            return (Get(x, y) & flag.AsShort()) != 0;
        }

        /**
		 * Gets the value of the specified tile.
		 *
		 * @param x The x coordinate of the tile.
		 * @param y The y coordinate of the tile.
		 * @return The value.
		 */
        public int Get(int x, int y)
        {
            return matrix[IndexOf(x, y)] & 0xFFFF;
        }

        /**
		 * Resets the cell of the specified coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 */
        public void Reset(int x, int y)
        {
            Set(x, y, All_Allowed);
        }

        /**
		 * Resets all cells in this matrix.
		 */
        public void Reset()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Reset(x, y);
                }
            }
        }

        /**
		 * Sets (i.e. sets to {@code true}) the value of the specified {@link CollisionFlag} for the specified coordinate
		 * pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param flag The CollisionFlag.
		 */
        public void Set(int x, int y, CollisionFlag flag)
        {
            Set(x, y, flag.AsShort());
        }

        /**
		 * Returns whether or not an Entity of the specified {@link EntityType type} cannot traverse the tile at the
		 * specified coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param entity The {@link EntityType}.
		 * @param direction The {@link Direction} the Entity is approaching from.
		 * @return {@code true} iff the tile at the specified coordinate pair is not traversable.
		 */
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

        /**
		 * Gets the index in the matrix for the specified coordinate pair.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @return The index.
		 * @throws ArrayIndexOutOfBoundsException If the specified coordinate pair does not fit in this matrix.
		 */
        private int IndexOf(int x, int y)
        {
            Guard.Argument(x).GreaterThan(0).LessThan(width);
            Guard.Argument(y).GreaterThan(0).LessThan(length);
            return y * width + x;
        }

        /**
		 * Sets the appropriate index for the specified coordinate pair to the specified value.
		 *
		 * @param x The x coordinate.
		 * @param y The y coordinate.
		 * @param value The value.
		 */
        private void Set(int x, int y, short value)
        {
            matrix[IndexOf(x, y)] = value;
        }

    }

}
