using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.World.Updating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model
{
    public abstract class Mob : Entity
    {
        [NotMapped] public Direction FirstDirection { get; set; } = Direction.None;
        [NotMapped] public Direction SecondDirection { get; set; } = Direction.None;
        [NotMapped] public Direction LastDirection { get; set; } = Direction.North;
        [NotMapped] public SynchronizationBlockSet BlockSet { get; set; } = new SynchronizationBlockSet();
        [NotMapped] public bool IsActive => Index != -1;
        [NotMapped] public bool IsTeleporting { get; set; }
        [NotMapped] public WalkingQueue WalkingQueue { get; set; } = new WalkingQueue();
        public Direction[] GetDirections()
        {
            if (FirstDirection != Direction.None)
            {
                return SecondDirection == Direction.None ? new Direction[] { FirstDirection }
                    : new Direction[] { FirstDirection, SecondDirection };
            }

            return Direction.EmptyDirectionArray;
        }
    }
}
