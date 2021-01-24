namespace NetScape.Abstractions.Model.Region
{
	/// <summary>
	/// An immutable class representing the coordinates of a region, where the coordinates ({@code x, y}) are the top-left of
	/// the region. 
	/// @author Graham
	/// @author Major
	/// </summary>
	public sealed class RegionCoordinates
	{

		/// <summary>
		/// Gets the RegionCoordinates for the specified <seealso cref="Position"/>.
		/// </summary>
		/// <param name="position"> The Position. </param>
		/// <returns> The RegionCoordinates. </returns>
		public static RegionCoordinates FromPosition(Position position)
		{
			return new RegionCoordinates(position.TopLeftRegionX, position.TopLeftRegionY);
		}

		/// <summary>
		/// Creates the RegionCoordinates.
		/// </summary>
		/// <param name="x"> The x coordinate. </param>
		/// <param name="y"> The y coordinate. </param>
		public RegionCoordinates(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj is RegionCoordinates)
			{
				RegionCoordinates other = (RegionCoordinates)obj;
				return X == other.X && Y == other.Y;
			}

			return false;
		}

		/// <summary>
		/// Gets the absolute x coordinate of this Region (which can be compared directly against <seealso cref="Position.X"/>.
		/// </summary>
		/// <returns> The absolute x coordinate. </returns>
		public int AbsoluteX
		{
			get
			{
				return Constants.RegionSize * (X + 6);
			}
		}

		/// <summary>
		/// Gets the absolute y coordinate of this Region (which can be compared directly against <seealso cref="Position.Y"/>.
		/// </summary>
		/// <returns> The absolute y coordinate. </returns>
		public int AbsoluteY
		{
			get
			{
				return Constants.RegionSize * (Y + 6);
			}
		}

		/// <summary>
		/// Gets the x coordinate (equivalent to the <seealso cref="Position.getTopLeftRegionX()"/> of a position within this region).
		/// </summary>
		/// <returns> The x coordinate. </returns>
		public int X { get; }

		/// <summary>
		/// Gets the y coordinate (equivalent to the <seealso cref="Position.getTopLeftRegionY()"/> of a position within this region).
		/// </summary>
		/// <returns> The y coordinate. </returns>
		public int Y { get; }

		public override int GetHashCode()
		{
			return X << 16 | Y;
		}

	}
}
