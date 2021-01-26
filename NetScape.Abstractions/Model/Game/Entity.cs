using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model.Game
{
    public abstract class Entity
    {

        [NotMapped]
        public int Index { get; set; }

        public Position Position { get; set; }

        [NotMapped]
		public abstract EntityType EntityType { get; }

        [NotMapped]
        public abstract int Width { get; }

        [NotMapped]
        public abstract int Length { get; }
    }
}
