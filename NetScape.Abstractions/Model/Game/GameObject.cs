using NetScape.Abstractions.Interfaces.Area;
using NetScape.Abstractions.Model.Area;
using NetScape.Abstractions.Model.Area.Obj;
using NetScape.Modules.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Game
{
    public abstract class GameObject : Entity, IGroupableEntity
    {

        public GameObject(Position pos)
        {
            this.Position = pos;
        }

        /**
         * The packed value that stores this object's id, type, and orientation.
         */
        private int _packed;

        /**
         * Creates the GameObject.
         *
         * @param world The {@link World} containing the GameObject.
         * @param id The id of the GameObject
         * @param position The {@link Position} of the GameObject.
         * @param type The type of the GameObject.
         * @param orientation The orientation of the GameObject.
         */
        public GameObject(int id, Position position, int type, int orientation) : this(position)
        {
            _packed = id << 8 | (type & 0x3F) << 2 | orientation & 0x3;
        }

        public override bool Equals(Object obj)
        {
            if (obj is GameObject)
            {
                GameObject other = (GameObject)obj;
                return Position.Equals(other.Position) && _packed == other._packed;
            }
            return false;
        }

        /**
         * Gets this object's id.
         *
         * @return The id.
         */
        public int Id => (int)((uint)_packed >> 8);

        /**
         * Gets this object's orientation.
         *
         * @return The orientation.
         */
        public int Orientation => _packed & 0x3;

        /**
         * Gets this object's type.
         *
         * @return The type.
         */
        public int Type => _packed >> 2 & 0x3F;


        public override int Length =>  /* isRotated() ? getDefinition().getWidth() : getDefinition().getLength()*/ 1;

        public override int Width => /*isRotated() ? getDefinition().getLength() : getDefinition().getWidth();*/ 1;

        /**
         * Returns whether or not this GameObject's orientation is rotated {@link Direction#WEST} or {@link Direction#EAST}.
         *
         * @return {@code true} iff this GameObject's orientation is rotated.
         */
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

        /**
         * Returns whether or not this GameObject can be seen by the specified {@link Player}.
         *
         * @param player The Player.
         * @param world The {@link World} containing the GameObject.
         * @return {@code true} if the Player can see this GameObject, {@code false} if not.
         */
        public abstract bool ViewableBy(Player player, IWorld world);

        public UpdateOperation ToUpdateOperation(Region region, EntityUpdateType type)
        {
            return new ObjectUpdateOperation(region, type, this);
        }
    }

}
