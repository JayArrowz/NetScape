using NetScape.Abstractions.Model.World.Updating;

namespace NetScape.Modules.ThreeOneSeven.World.Updating.Segments
{
    public abstract class SynchronizationSegment
    {
        public SynchronizationBlockSet BlockSet { get; }

        public SynchronizationSegment(SynchronizationBlockSet blockSet)
        {
            BlockSet = blockSet;
        }
        
        public abstract SegmentType Type { get; }
    }
}
