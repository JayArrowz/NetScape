using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Model.World.Updating.Blocks
{
    public class AppearanceBlock : SynchronizationBlock
    {
        public Appearance Appearance { get; set; }
        public int Combat { get; set; }
        public bool IsSkulled { get; set; }
        public long Name { get; set; }
        public int NpcId { get; set; }
        public int HeadIcon { get; set; }
        public int Skill { get; set; }

		public AppearanceBlock(long name, Appearance appearance, int combat, int skill, int headIcon, bool isSkulled) : this(name, appearance, combat, skill, headIcon, isSkulled, -1)
		{
		}

		public  AppearanceBlock(long name, Appearance appearance, int combat, int skill, int headIcon, bool isSkulled, int npcId)
		{
			Name = name;
			Appearance = appearance;
			Combat = combat;
			Skill = skill;
			//this.equipment = equipment.duplicate();
			HeadIcon = headIcon;
			IsSkulled = isSkulled;
			NpcId = npcId;
		}
	}
}
