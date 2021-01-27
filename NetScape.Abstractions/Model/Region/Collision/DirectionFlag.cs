namespace NetScape.Abstractions.Model.Region.Collision
{
    /// <summary>
    /// A directional flag in a <see cref="Interfaces.Region.Collision.ICollisionUpdate"/>. Consists of a <see cref="Direction"/> and a flag indicating whether 
    /// that tile is impenetrable as well as untraversable.
    /// </summary>
    public record DirectionFlag
    {
        public bool Impenetrable { get; }
        public Direction Direction { get; }

        public DirectionFlag(bool impenetrable, Direction direction)
        {
            Impenetrable = impenetrable;
            Direction = direction;
        }
    }
}
