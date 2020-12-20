using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model.Game
{
    public class Entity
    {
        [NotMapped]
        public int Index { get; set; }

        public Position Position { get; set; }
    }
}
