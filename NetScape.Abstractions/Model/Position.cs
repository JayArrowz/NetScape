using Microsoft.EntityFrameworkCore;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Model.Region;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model
{
    /// <summary>
    /// Represents a position in the world
    /// @author Graham
    /// </summary>
    [Owned]
    public class Position
    {

        /// <summary>
        /// The number of height levels, (0, 3] inclusive.
        /// </summary>
        public const int HeightLevels = 4;

        /// <summary>
        /// The maximum distance players/NPCs can 'see'.
        /// </summary>
        public const int MaxDistance = 15;

        /// <summary>
        /// Creates a position at the default height.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        public Position(int x, int y) : this(x, y, 0)
        {
        }

        public Position() : this(0, 0, 0)
        {

        }

        /// <summary>
        /// Creates a position with the specified height.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <param name="height"> The height. </param>
        public Position(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Position)
            {
                Position other = (Position)obj;
                return GetHashCode() == other.GetHashCode();
            }

            return false;
        }

        /// <summary>
        /// Gets the x coordinate of the central region.
        /// </summary>
        /// <returns> The x coordinate of the central region. </returns>
        [NotMapped]
        public int CentralRegionX
        {
            get
            {
                return X / 8;
            }
        }

        /// <summary>
        /// Gets the y coordinate of the central region.
        /// </summary>
        /// <returns> The y coordinate of the central region. </returns>
        [NotMapped]
        public int CentralRegionY
        {
            get
            {
                return Y / 8;
            }
        }

        /// <summary>
        /// Gets the distance between this position and another position. Only x and y are considered (i.e. 2 dimensions).
        /// </summary>
        /// <param name="other"> The other position. </param>
        /// <returns> The distance. </returns>
        public int GetDistance(Position other)
        {
            int deltaX = X - other.X;
            int deltaY = Y - other.Y;
            return (int)Math.Ceiling(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
        }

        /// <summary>
        /// Gets the height level.
        /// </summary>
        /// <returns> The height level. </returns>
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the x coordinate inside the region of this position.
        /// </summary>
        /// <returns> The local x coordinate. </returns>
        [NotMapped]
        public int LocalX
        {
            get
            {
                return GetLocalX(this);
            }
        }

        /// <summary>
        /// Gets the local x coordinate inside the region of the {@code base} position.
        /// </summary>
        /// <param name="base"> The base position. </param>
        /// <returns> The local x coordinate. </returns>
        public int GetLocalX(Position @base)
        {
            return X - @base.TopLeftRegionX * 8;
        }

        /// <summary>
        /// Gets the y coordinate inside the region of this position.
        /// </summary>
        /// <returns> The local y coordinate. </returns>
        [NotMapped]
        public int LocalY
        {
            get
            {
                return GetLocalY(this);
            }
        }

        /// <summary>
        /// Gets the local y coordinate inside the region of the {@code base} position.
        /// </summary>
        /// <param name="base"> The base position. </param>
        /// <returns> The local y coordinate. </returns>
        public int GetLocalY(Position @base)
        {
            return Y - @base.TopLeftRegionY * 8;
        }

        /// <summary>
        /// Gets the longest horizontal or vertical delta between the two positions.
        /// </summary>
        /// <param name="other"> The other position. </param>
        /// <returns> The longest horizontal or vertical delta. </returns>
        public int GetLongestDelta(Position other)
        {
            int deltaX = Math.Abs(X - other.X);
            int deltaY = Math.Abs(Y - other.Y);
            return Math.Max(deltaX, deltaY);
        }

        /// <summary>
        /// Returns the <seealso cref="RegionCoordinates"/> of the <seealso cref="Region"/> this position is inside.
        /// </summary>
        /// <returns> The region coordinates. </returns>
        [NotMapped]
        public RegionCoordinates RegionCoordinates
        {
            get
            {
                return RegionCoordinates.FromPosition(this);
            }
        }

        /// <summary>
        /// Gets the x coordinate of the region this position is in.
        /// </summary>
        /// <returns> The region x coordinate. </returns>
        [NotMapped]
        public int TopLeftRegionX
        {
            get
            {
                return X / 8 - 6;
            }
        }

        /// <summary>
        /// Gets the y coordinate of the region this position is in.
        /// </summary>
        /// <returns> The region y coordinate. </returns>
        [NotMapped]
        public int TopLeftRegionY
        {
            get
            {
                return Y / 8 - 6;
            }
        }

        /// <summary>
        /// Gets the region x.
        /// </summary>
        /// <value>
        /// The region x.
        /// </value>
        [NotMapped]
        public int RegionX => (X >> 3) - 6;

        /// <summary>
        /// Gets the region y.
        /// </summary>
        /// <value>
        /// The region y.
        /// </value>
        [NotMapped]
        public int RegionY => (Y >> 3) - 6;

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <returns> The x coordinate. </returns>
        public int X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <returns> The y coordinate. </returns>
        public int Y
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            return Height << 30 | (Y & 0x7FFF) << 15 | X & 0x7FFF;
        }

        /// <summary>
        /// Returns whether or not this position is inside the specified <seealso cref="Region"/>.
        /// </summary>
        /// <param name="region"> The region. </param>
        /// <returns> {@code true} if this position is inside the specified region, otherwise {@code false}. </returns>
        public bool Inside(IRegion region)
        {
            RegionCoordinates coordinates = region.Coordinates;
            return coordinates.Equals(RegionCoordinates);
        }

        /// <summary>
        /// Checks if the position is within distance of another.
        /// </summary>
        /// <param name="other"> The other position. </param>
        /// <param name="distance"> The distance. </param>
        /// <returns> {@code true} if so, {@code false} if not. </returns>
        public bool IsWithinDistance(Position other, int distance)
        {
            int deltaX = Math.Abs(X - other.X);
            int deltaY = Math.Abs(Y - other.Y);
            return deltaX <= distance && deltaY <= distance && Height == other.Height;
        }

        /// <summary>
        /// Creates a new position {@code num} steps from this position in the given direction.
        /// </summary>
        /// <param name="num"> The number of steps to make. </param>
        /// <param name="direction"> The direction to make steps in. </param>
        /// <returns> A new {@code Position} that is {@code num} steps in {@code direction} ahead of this one. </returns>
        public Position Step(int num, Direction direction)
        {
            return new Position(X + (num * direction.DeltaX()), Y + (num * direction.DeltaY()), Height);
        }
    }
}