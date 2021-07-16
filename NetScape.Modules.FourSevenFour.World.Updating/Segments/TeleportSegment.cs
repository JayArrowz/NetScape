using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.World.Updating;

namespace NetScape.Modules.FourSevenFour.World.Updating.Segments
{
    public class TeleportSegment : SynchronizationSegment
    {
        public Position Destination { get; }
        public TeleportSegment(SynchronizationBlockSet blockSet, Position dest) : base(blockSet)
        {
            Destination = dest;
        }

        public override SegmentType Type => SegmentType.Teleport;
    }
}
