using NetScape.Abstractions.Model.World.Updating;

namespace NetScape.Modules.FourSevenFour.World.Updating.Segements
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
