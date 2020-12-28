using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.World.Updating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.World.Updating.Segments
{
    public class AddPlayerSegment : SynchronizationSegment
    {
        public override SegmentType Type => SegmentType.Add_Mob;
        public int Index { get; }
        public Position Position { get; }

        public AddPlayerSegment(SynchronizationBlockSet blockSet, int index, Position position) : base(blockSet)
        {
            Index = index;
            Position = position;
        }
    }
}
