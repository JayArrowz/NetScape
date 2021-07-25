using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetScape.Abstractions.Model.Game
{
    public record Appearance
    {
        public static Appearance DefaultAppearance => new Appearance(Gender.Male, new int[] { 0, 10, 18, 26, 33, 36, 42 }, new int[5]);

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int[] Colors { get; set; }

        [DataType("varchar(20)")]
        public Gender Gender { get; set; }
        public int[] Style { get; set; }

        public Appearance()
        {

        }

        public Appearance(Gender gender, int[] style, int[] colors)
        {
            if (style == null || colors == null)
            {
                throw new ArgumentException("No arguments can be null.");
            }

            Gender = gender;
            Style = style;
            Colors = colors;
        }

        [NotMapped]
        public bool IsFemale => Gender == Gender.Female;

        [NotMapped]
        public bool IsMale => Gender == Gender.Male;
    }
}
