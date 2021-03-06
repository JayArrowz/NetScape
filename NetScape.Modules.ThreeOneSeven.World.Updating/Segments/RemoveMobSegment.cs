﻿using NetScape.Abstractions.Model.World.Updating;

namespace NetScape.Modules.ThreeOneSeven.World.Updating.Segments
{
    public class RemoveMobSegment : SynchronizationSegment
    {
        private static readonly SynchronizationBlockSet EmptyBlockSet = new SynchronizationBlockSet();

        public RemoveMobSegment() : base(EmptyBlockSet)
        {

        }

        public override SegmentType Type => SegmentType.Remove_Mob;
    }
}
