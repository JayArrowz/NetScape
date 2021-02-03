using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model.Game
{
    public partial class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public Appearance Appearance { get; set; }
        public int AppearanceId { get; set; }
    }
}
