using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Interfaces.World;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;

namespace NetScape.Modules.Region
{
    public abstract class GameObject : Entity, IGroupableEntity
    {

        public GameObject(Position pos)
        {
            this.Position = pos;
        }

        /// <summary>
        /// The packed value that stores this object's id, type, and orientation.
        /// </summary>
        private int _packed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObject"/> class.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        /// <param name="orientation">The orientation.</param>
        public GameObject(int id, Position position, int type, int orientation) : this(position)
        {
            _packed = id << 8 | (type & 0x3F) << 2 | orientation & 0x3;
        }

        public override bool Equals(object obj)
        {
            if (obj is GameObject)
            {
                GameObject other = (GameObject)obj;
                return Position.Equals(other.Position) && _packed == other._packed;
            }
            return false;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id => (int)((uint)_packed >> 8);

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public int Orientation => _packed & 0x3;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public int Type => _packed >> 2 & 0x3F;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override int Length =>  /* isRotated() ? getDefinition().getWidth() : getDefinition().getLength()*/ 1;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public override int Width => /*isRotated() ? getDefinition().getLength() : getDefinition().getWidth();*/ 1;

        /// <summary>
        /// Determines whether this instance is rotated.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is rotated  <see cref="Direction.West"/> or  <see cref="Direction.East"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRotated()
        {
            int orientation = Orientation;
            int type = Type;
            Direction direction = Direction.WNES[orientation];

            if (type == ObjectType.Triangular_Corner.Value || type == ObjectType.Rectangular_Corner.Value)
            {
                direction = Direction.WNES_DIAGONAL[orientation];
            }

            return direction == Direction.North || direction == Direction.South
                || direction == Direction.NorthWest || direction == Direction.SouthEast;
        }

        public override int GetHashCode()
        {
            return _packed;
        }

        /// <summary>
        /// Returns whether or not this GameObject can be seen by the specified player
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="world">The world.</param>
        /// <returns> <c>true</c> if the Player can see this GameObject, <c>false</c> if not</returns>
        public abstract bool ViewableBy(Player player, IWorld world);

        public IRegionUpdateOperation ToUpdateOperation(IRegion region, EntityUpdateType type)
        {
            return new ObjectUpdateOperation(region, type, this);
        }
    }

}
