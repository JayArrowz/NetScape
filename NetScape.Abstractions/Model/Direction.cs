using System.Collections.Generic;

namespace NetScape.Abstractions.Model
{
	public sealed class Direction
	{
		public static readonly Direction None = new Direction("NONE", InnerDirectionValue.None, -1);
		public static readonly Direction NorthWest = new Direction("NORTH_WEST", InnerDirectionValue.NorthWest, 0);
		public static readonly Direction North = new Direction("NORTH", InnerDirectionValue.North, 1);
		public static readonly Direction NorthEast = new Direction("NORTH_EAST", InnerDirectionValue.NorthEast, 2);
		public static readonly Direction West = new Direction("WEST", InnerDirectionValue.West, 3);
		public static readonly Direction East = new Direction("EAST", InnerDirectionValue.East, 4);
		public static readonly Direction SouthWest = new Direction("SOUTH_WEST", InnerDirectionValue.SouthWest, 5);
		public static readonly Direction South = new Direction("SOUTH", InnerDirectionValue.South, 6);
		public static readonly Direction SouthEast = new Direction("SOUTH_EAST", InnerDirectionValue.SouthEast, 7);

		private static List<Direction> Values { get; } = new List<Direction>();

		static Direction()
		{
			Values.Add(None);
			Values.Add(NorthWest);
			Values.Add(North);
			Values.Add(NorthEast);
			Values.Add(West);
			Values.Add(East);
			Values.Add(SouthWest);
			Values.Add(South);
			Values.Add(SouthEast);
		}

		public enum InnerDirectionValue
		{
			None,
			NorthWest,
			North,
			NorthEast,
			West,
			East,
			SouthWest,
			South,
			SouthEast
		}

		public InnerDirectionValue Value { get; }
		private readonly string _nameValue;
		private readonly int _ordinalValue;
		private static int _nextOrdinal = 0;

		/// <summary>
		/// An empty direction array.
		/// </summary>
		public static readonly Direction[] EMPTY_DIRECTION_ARRAY = new Direction[0];

		/// <summary>
		/// An array of directions without any diagonal directions.
		/// </summary>
		public static readonly Direction[] NESW = new Direction[] { North, East, South, West };

		/// <summary>
		/// An array of directions without any diagonal directions, and one step counter-clockwise, as used by
		/// the clients collision mapping.
		/// </summary>
		public static readonly Direction[] WNES = new Direction[] { West, North, East, South };

		/// <summary>
		/// An array of diagonal directions, and one step counter-clockwise, as used by the clients collision
		/// mapping.
		/// </summary>
		public static readonly Direction[] WNES_DIAGONAL = new Direction[] { NorthWest, NorthEast, SouthEast, SouthWest };

		/// <summary>
		/// Gets the Direction between the two <seealso cref="Position"/>s..
		/// </summary>
		/// <param name="current"> The difference between two X coordinates. </param>
		/// <param name="next"> The difference between two Y coordinates. </param>
		/// <returns> The direction. </returns>
		public static Direction Between(Position current, Position next)
		{
			int deltaX = next.X - current.X;
			int deltaY = next.Y - current.Y;

			return FromDeltas(deltaX, deltaY);
		}

		/// <summary>
		/// Creates a direction from the differences between X and Y.
		/// </summary>
		/// <param name="deltaX"> The difference between two X coordinates. </param>
		/// <param name="deltaY"> The difference between two Y coordinates. </param>
		/// <returns> The direction. </returns>
		public static Direction FromDeltas(int deltaX, int deltaY)
		{
			if (deltaY == 1)
			{
				if (deltaX == 1)
				{
					return NorthEast;
				}
				else if (deltaX == 0)
				{
					return North;
				}
				else if (deltaX == -1)
				{
					return NorthWest;
				}
			}
			else if (deltaY == -1)
			{
				if (deltaX == 1)
				{
					return SouthEast;
				}
				else if (deltaX == 0)
				{
					return South;
				}
				else if (deltaX == -1)
				{
					return SouthWest;
				}
			}
			else if (deltaY == 0)
			{
				if (deltaX == 1)
				{
					return East;
				}
				else if (deltaX == 0)
				{
					return None;
				}
				else if (deltaX == -1)
				{
					return West;
				}
			}

			throw new System.ArgumentException("Difference between Positions must be [-1, 1].");
		}

		/// <summary>
		/// Get the 2 directions which make up a diagonal direction (i.e., NORTH and EAST for NORTH_EAST).
		/// </summary>
		/// <param name="direction"> The direction to get the components for. </param>
		/// <returns> The components for the given direction. </returns>
		public static Direction[] DiagonalComponents(Direction direction)
		{
			switch (direction.Value)
			{
				case InnerDirectionValue.NorthEast:
					return new Direction[] { North, East };
				case InnerDirectionValue.NorthWest:
					return new Direction[] { North, West };
				case InnerDirectionValue.SouthEast:
					return new Direction[] { South, East };
				case InnerDirectionValue.SouthWest:
					return new Direction[] { South, West };
			}

			throw new System.ArgumentException("Must provide a diagonal direction.");
		}

		/// <summary>
		/// The direction as an integer.
		/// </summary>
		private readonly int intValue;

		/// <summary>
		/// Creates the direction.
		/// </summary>
		/// <param name="intValue"> The direction as an integer. </param>
		internal Direction(string name, InnerDirectionValue innerEnum, int intValue)
		{
			this.intValue = intValue;

			_nameValue = name;
			_ordinalValue = _nextOrdinal++;
			Value = innerEnum;
		}

		/// <summary>
		/// Gets the opposite direction of the this direction.
		/// </summary>
		/// <returns> The opposite direction. </returns>
		public Direction Opposite()
		{
			switch (Value)
			{
				case InnerDirectionValue.North:
					return South;
				case InnerDirectionValue.South:
					return North;
				case InnerDirectionValue.East:
					return West;
				case InnerDirectionValue.West:
					return East;
				case InnerDirectionValue.NorthWest:
					return SouthEast;
				case InnerDirectionValue.NorthEast:
					return SouthWest;
				case InnerDirectionValue.SouthEast:
					return NorthWest;
				case InnerDirectionValue.SouthWest:
					return NorthEast;
			}
			return None;
		}

		/// <summary>
		/// Gets the X delta from a <seealso cref="Position"/> of (0, 0).
		/// </summary>
		/// <returns> The delta of X from (0, 0). </returns>
		public int DeltaX()
		{
			switch (Value)
			{
				case InnerDirectionValue.SouthEast:
				case InnerDirectionValue.NorthEast:
				case InnerDirectionValue.East:
					return 1;
				case InnerDirectionValue.SouthWest:
				case InnerDirectionValue.NorthWest:
				case InnerDirectionValue.West:
					return -1;
			}

			return 0;
		}

		/// <summary>
		/// Gets the Y delta from a <seealso cref="Position"/> of (0, 0).
		/// </summary>
		/// <returns> The delta of Y from (0, 0). </returns>
		public int DeltaY()
		{
			switch (Value)
			{
				case InnerDirectionValue.NorthWest:
				case InnerDirectionValue.NorthEast:
				case InnerDirectionValue.North:
					return 1;
				case InnerDirectionValue.SouthWest:
				case InnerDirectionValue.SouthEast:
				case InnerDirectionValue.South:
					return -1;
			}

			return 0;
		}

		/// <summary>
		/// Check if this direction is a diagonal direction.
		/// </summary>
		/// <returns> {@code true} if this direction is a diagonal direction, {@code false} otherwise. </returns>
		public bool Diagonal
		{
			get
			{
				return this == SouthEast || this == SouthWest || this == NorthEast || this == NorthWest;
			}
		}

		/// <summary>
		/// Gets the direction as an integer which the client can understand.
		/// </summary>
		/// <returns> The movement as an integer. </returns>
		public int toInteger()
		{
			return intValue;
		}


		/// <summary>
		/// Gets the direction as an integer as used orientation in the client maps (WNES as opposed to NESW).
		/// </summary>
		/// <returns> The direction as an integer. </returns>
		public int toOrientationInteger()
		{
			switch (Value)
			{
				case InnerDirectionValue.West:
				case InnerDirectionValue.NorthWest:
					return 0;
				case InnerDirectionValue.North:
				case InnerDirectionValue.NorthEast:
					return 1;
				case InnerDirectionValue.East:
				case InnerDirectionValue.SouthEast:
					return 2;
				case InnerDirectionValue.South:
				case InnerDirectionValue.SouthWest:
					return 3;
				default:
					throw new System.InvalidOperationException("Only a valid direction can have an orientation value");
			}

		}

		public static Direction[] values()
		{
			return Values.ToArray();
		}

		public int Ordinal()
		{
			return _ordinalValue;
		}

		public override string ToString()
		{
			return _nameValue;
		}

		public static Direction valueOf(string name)
		{
			foreach (Direction enumInstance in Direction.Values)
			{
				if (enumInstance._nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}

	}
}
