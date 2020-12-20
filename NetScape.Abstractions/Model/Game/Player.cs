using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model.Game
{
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
