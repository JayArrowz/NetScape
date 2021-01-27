using Microsoft.Collections.Extensions;
using NetScape.Abstractions.Interfaces.Region.Collision;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Region.Collision;
using static NetScape.Abstractions.Model.Region.ObjectType;

namespace NetScape.Modules.Region.Collision
{
    /// <seealso cref="ICollisionUpdate" />
    public class CollisionUpdate : ICollisionUpdate
    {
        /// <summary>
        /// Gets or sets the type of update.
        /// </summary>
        /// <value>
        /// The type of this update.
        /// </value>
        public CollisionUpdateType Type { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>
        /// A mapping of <see cref="T:NetScape.Abstractions.Model.Position" />s to their <see cref="T:NetScape.Abstractions.Model.Region.Collision.DirectionFlag" />s.
        /// </value>
        public MultiValueDictionary<Position, DirectionFlag> Flags { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionUpdate"/> class.
        /// </summary>
        /// <param name="type">The <see cref="CollisionUpdateType"/> of this update.</param>
        /// <param name="flags">The <see cref="Flags"/> of <see cref="Position"/>s to their <see cref="DirectionFlag"/>s.</param>
        public CollisionUpdate(CollisionUpdateType type, MultiValueDictionary<Position, DirectionFlag> flags)
        {
            Type = type;
            Flags = flags;
        }

        /// <summary>
        /// A builder for CollisionUpdates.
        /// </summary>
        public class Builder
        {

            private readonly MultiValueDictionary<Position, DirectionFlag> _flags;
            public CollisionUpdateType Type { get; set; }

            public Builder()
            {
                _flags = new();
            }

            /// <summary>
            /// Sets the tile at the given <paramref name="position"/> as untraversable in the given directions.
            /// </summary>
            /// <param name="position">The world position of the tile.</param>
            /// <param name="impenetrable">if set to <c>true</c> [impenetrable].</param>
            /// <param name="directions">The directions that are untraversable from this tile.</param>          
            public void Tile(Position position, bool impenetrable, params Direction[] directions)
            {
                if (directions.Length == 0)
                {
                    return;
                }

                foreach (var direction in directions)
                {
                    _flags.Add(position, new DirectionFlag(impenetrable, direction));
                }
            }

            /// <summary>
            /// Flag a wall in the CollisionUpdate.  When constructing a CollisionMatrix, the flags for a wall are represented
            /// as the tile the wall exists on and the tile one step in the facing direction. So for a wall facing south,
            /// the tile one step to the south be flagged as untraversable from the north
            /// </summary>
            /// <param name="position">The position of the wall.</param>
            /// <param name="impenetrable">if set to <c>true</c> [projectiles can pass through this wall].</param>
            /// <param name="orientation">The facing direction of this wall.</param>          
            public void Wall(Position position, bool impenetrable, Direction orientation)
            {
                Tile(position, impenetrable, orientation);
                Tile(position.Step(1, orientation), impenetrable, orientation.Opposite());
            }

            /// <summary>
            /// Flag a larger corner wall in the CollisionUpdate.  A corner is represented by the 2 directions that it faces,
            /// and the 2 tiles in both directions.For example, when a tile is facing north its facing directions
            /// are north and east, so the position of the object will be untraversable from those directions.Additionally,
            /// the tile 1 step to the north, and 1 step to the east will be untraversable from the opposite directions of
            /// north and east respectively.
            /// </summary>
            /// <param name="position">The position of the corner wall..</param>
            /// <param name="impenetrable">if set to <c>true</c> [projectiles can pass through this corner wall].</param>
            /// <param name="orientation">The direction of this corner wall.</param>
            public void LargeCornerWall(Position position, bool impenetrable, Direction orientation)
            {
                Direction[] directions = Direction.DiagonalComponents(orientation);
                Tile(position, impenetrable, directions);

                foreach (Direction direction in directions)
                {
                    Tile(position.Step(1, direction), impenetrable, direction.Opposite());
                }
            }

            /// <summary>
            /// Flag a collision update for the given <paramref name="gameObject"/>.
            /// </summary>
            /// <param name="gameObject">The game object.</param>
            public void Object(GameObject gameObject)
            {
                //ObjectDefinition definition = gameObject.Definition;
                Position position = gameObject.Position;
                int type = gameObject.Type;


                /*if (!unwalkable(definition, type)) {
                    return;
                }*/

                int x = position.X, y = position.Y, height = position.Height;
                var impenetrable = /*definition.isImpenetrable()*/ false;
                int orientation = gameObject.Orientation;

                if (type == Floor_Decoration.Value)
                {
                    if (/*definition.isInteractive() && definition.isSolid()*/ false)
                    {
                        Tile(new Position(x, y, height), impenetrable, Direction.NESW);
                    }
                }
                else if (type >= Diagonal_Wall.Value && type < Floor_Decoration.Value)
                {
                    for (int dx = 0; dx < gameObject.Width; dx++)
                    {
                        for (int dy = 0; dy < gameObject.Length; dy++)
                        {
                            Tile(new Position(x + dx, y + dy, height), impenetrable, Direction.NESW);
                        }
                    }
                }
                else if (type == Lengthwise_Wall.Value)
                {
                    Wall(position, impenetrable, Direction.WNES[orientation]);
                }
                else if (type == Triangular_Corner.Value || type == Rectangular_Corner.Value)
                {
                    Wall(position, impenetrable, Direction.WNES_DIAGONAL[orientation]);
                }
                else if (type == Wall_Corner.Value)
                {
                    LargeCornerWall(position, impenetrable, Direction.WNES_DIAGONAL[orientation]);
                }
            }

            /// <summary>
            /// Create a new <see cref="ICollisionUpdate"/>.
            /// </summary>
            /// <returns>A new CollisionUpdate with the flags in this builder.</returns>         
            public ICollisionUpdate Build()
            {
                return new CollisionUpdate(Type, new MultiValueDictionary<Position, DirectionFlag>(_flags));
            }
        }

        /// <summary>
        /// Returns whether or not an object with the specified {@link ObjectDefinition} and <paramref name="type"/> should result in
        /// the tile(s) it is located on being blocked.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <returns><c>true</c> if [the tile(s) the object is on should be blocked] otherwise <c>false</c></returns> 
        private static bool Unwalkable(/*ObjectDefinition definition,*/ int type)
        {
            bool isSolidFloorDecoration = /*type == Floor_Decoration.Value && definition.isInteractive()*/ false;
            bool isRoof = type > Diagonal_Interactable.Value && type < Floor_Decoration.Value;

            bool isWall = type >= Lengthwise_Wall.Value && type <= Rectangular_Corner.Value ||
                type == Diagonal_Wall.Value;

            bool isSolidInteractable = /*(type == Diagonal_Interactable.Value ||
                type == Interactable.Value) && definition.isSolid()*/ false;

            return isWall || isRoof || isSolidInteractable || isSolidFloorDecoration;
        }
    }
}
