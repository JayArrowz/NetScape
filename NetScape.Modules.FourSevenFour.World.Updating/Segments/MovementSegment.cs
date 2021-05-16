using Dawn;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.World.Updating;
using System;

namespace NetScape.Modules.FourSevenFour.World.Updating.Segements
{
    public class MovementSegment : SynchronizationSegment
    {
        public Direction[] Directions { get; }
        public override SegmentType Type => GetSegmentType();

        public MovementSegment(SynchronizationBlockSet blockSet, Direction[] directions) : base(blockSet)
        {
            Guard.Argument(directions.Length, nameof(Directions)).GreaterThan(-1).LessThan(3);
            Directions = directions;
        }

        private SegmentType GetSegmentType()
        {
            switch (Directions.Length)
            {
                case 0:
                    return SegmentType.No_Movement;
                case 1:
                    return SegmentType.Walk;
                case 2:
                    return SegmentType.Run;
                default:
                    throw new InvalidOperationException("Direction type not supported");
            }
        }
    }
}
