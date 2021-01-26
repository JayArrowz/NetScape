using DotNetty.Transport.Channels;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Game
{
    public partial class Player : Entity
    {
        private static readonly int DefaultViewingDistance = 15;
        private static int appearanceTicketCounter = 0;

        [NotMapped] public Direction FirstDirection { get; set; } = Direction.None;
        [NotMapped] public Direction SecondDirection { get; set; } = Direction.None;
        [NotMapped] public Position LastKnownRegion { get; set; }
        [NotMapped] public bool RegionChanged { get; set; }
        [NotMapped] public SynchronizationBlockSet BlockSet { get; set; } = new SynchronizationBlockSet();
        [NotMapped] public int AppearanceTicket { get; } = NextAppearanceTicket();
        [NotMapped] public bool IsTeleporting { get; set; }
        [NotMapped] public int ViewingDistance { get; private set; }
        [NotMapped] public IChannelHandlerContext ChannelHandlerContext { get; set; }
        [NotMapped] public bool Initialized { get; set; }
        [NotMapped] public override EntityType EntityType => EntityType.Player;
        [NotMapped] public override int Width => 1;
        [NotMapped] public override int Length => 1;
        [NotMapped] public List<Player> LocalPlayerList { get; set; } = new();
        [NotMapped] public bool IsActive => Index != -1;
        [NotMapped] public int[] AppearanceTickets { get; set; } = new int[2000];
        [NotMapped] public bool ExcessivePlayers { get; set; }

        public void IncrementViewingDistance()
        {
            if (ViewingDistance < Position.MaxDistance)
            {
                ViewingDistance++;
            }
        }

        public void DecrementViewingDistance()
        {
            if (ViewingDistance > 1)
            {
                ViewingDistance--;
            }
        }
        public void UpdateAppearance()
        {
            BlockSet.Add(SynchronizationBlock.CreateAppearanceBlock(this));
        }

        public void SendInitialMessages()
        {
            UpdateAppearance();
            //send(new IdAssignmentMessage(index, members));
        }


        private static int NextAppearanceTicket()
        {
            return Interlocked.Increment(ref appearanceTicketCounter);
        }

        public Direction[] GetDirections()
        {
            if (FirstDirection != Direction.None)
            {
                return SecondDirection == Direction.None ? new Direction[] { FirstDirection }
                    : new Direction[] { FirstDirection, SecondDirection };
            }

            return Direction.EmptyDirectionArray;
        }

        public bool HasLastKnownRegion()
        {
            return LastKnownRegion != null;
        }

        public void ResetViewingDistance()
        {
            ViewingDistance = DefaultViewingDistance;
        }

        /// <summary>
        /// Sends a message to the player.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendAsync(IOutMessage<MessageFrame> message)
        {
            var msg = message.ToMessage(ChannelHandlerContext.Allocator);
            await ChannelHandlerContext.Channel.WriteAndFlushAsync(msg);
        }
    }
}
