using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model.Game
{
    public partial class Player
    {
        private Appearance appearance;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public Appearance Appearance { get => appearance; set { appearance = value; UpdateAppearance(); } }
        public int AppearanceId { get; set; }
    }
}
