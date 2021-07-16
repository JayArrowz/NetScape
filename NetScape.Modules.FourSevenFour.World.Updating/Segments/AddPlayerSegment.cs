using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.World.Updating;

namespace NetScape.Modules.FourSevenFour.World.Updating.Segments
{
    public class AddPlayerSegment : SynchronizationSegment
    {
        public override SegmentType Type => SegmentType.Add_Mob;
        public int Index { get; }
        public Position Position { get; }
        public Direction Direction { get; }
        public AddPlayerSegment(SynchronizationBlockSet blockSet, int index, Position position, Direction direction) : base(blockSet)
        {
            Index = index;
            Position = position;
        }
    }
}
