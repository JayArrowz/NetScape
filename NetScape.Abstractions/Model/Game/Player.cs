using DotNetty.Transport.Channels;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Game
{
    public class Player : Entity
    {
        private static readonly int DefaultViewingDistance = 15;
        private static int appearanceTicketCounter = 0;
        private Appearance appearance;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public Appearance Appearance { get => appearance; set { appearance = value; UpdateAppearance(); } }
        public int AppearanceId { get; set; }

        [NotMapped] public Direction FirstDirection { get; set; }
        [NotMapped] public Direction SecondDirection { get; set; }
        [NotMapped] public Position LastKnownRegion { get; set; }
        [NotMapped] public bool RegionChanged { get; set; }
        [NotMapped] public SynchronizationBlockSet BlockSet { get; set; } = new SynchronizationBlockSet();
        [NotMapped] public int AppearanceTicket { get; } = NextAppearanceTicket();
        [NotMapped] public bool IsTeleporting { get; set; }
        [NotMapped] public int ViewingDistance { get; private set; }
        [NotMapped] public IChannelHandlerContext ChannelHandlerContext { get; set; }
        [NotMapped] public override EntityType EntityType => EntityType.Player;
        [NotMapped] public override int Width => 1;
        [NotMapped] public override int Length => 1;

        public void UpdateAppearance()
        {
            BlockSet.Add(SynchronizationBlock.CreateAppearanceBlock(this));
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
            await ChannelHandlerContext.WriteAndFlushAsync(message.ToMessage(ChannelHandlerContext.Allocator));
        }
    }
}
