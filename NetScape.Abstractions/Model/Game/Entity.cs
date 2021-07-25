using System.ComponentModel.DataAnnotations.Schema;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Interfaces.World;

namespace NetScape.Abstractions.Model.Game
{
    public abstract class Entity
    {
        private Position _position;

        [NotMapped]
        public IWorld World { get; set; }

        [NotMapped]
        public int Index { get; set; }

        public Position Position
        {
            get => _position;
            set
            {
                if (World != null && _position != value)
                {
                    Position old = _position;
                    var repository = World.RegionRepository;
                    IRegion current = repository.FromPosition(old), next = repository.FromPosition(value);
                    current.RemoveEntity(this);
                    _position = value;
                    next.AddEntity(this);
                } else if (World == null)
                {
                    _position = value;
                }
            }
        }

        [NotMapped]
        public abstract EntityType EntityType { get; }

        [NotMapped]
        public abstract int Width { get; }

        [NotMapped]
        public abstract int Length { get; }
    }
}
