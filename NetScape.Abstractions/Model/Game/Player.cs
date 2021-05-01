using DotNetty.Transport.Channels;
using Google.Protobuf;
using NetScape.Abstractions.Model.Messages;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Abstractions.Model.World.Updating.Blocks;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Game
{
    public partial class Player : Mob
    {
        private static readonly int DefaultViewingDistance = 15;
        private static int _appearanceTicketCounter = 0;

        [NotMapped] public Position LastKnownRegion { get; set; }
        [NotMapped] public bool RegionChanged { get; set; }
        [NotMapped] public int AppearanceTicket { get; } = NextAppearanceTicket();
        [NotMapped] public int ViewingDistance { get; private set; }
        [NotMapped] public IChannelHandlerContext ChannelHandlerContext { get; set; }
        [NotMapped] public bool Initialized { get; set; }
        [NotMapped] public override EntityType EntityType => EntityType.Player;
        [NotMapped] public override int Width => 1;
        [NotMapped] public override int Length => 1;
        [NotMapped] public List<Player> LocalPlayerList { get; set; } = new();
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

        private static int NextAppearanceTicket()
        {
            return Interlocked.Increment(ref _appearanceTicketCounter);
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
        public async Task SendAsync(IEncoderMessage<MessageFrame> message)
        {
            var msg = message.ToMessage(ChannelHandlerContext.Allocator);

            if (ChannelHandlerContext.Channel.Active)
            {
                await ChannelHandlerContext.Channel.WriteAndFlushAsync(msg);
            }
        }

        public void UpdateAppearance()
        {
            BlockSet.Add(SynchronizationBlock.CreateAppearanceBlock(this));
        }

        public void SendAnimation(Animation animation)
        {
            BlockSet.Add(SynchronizationBlock.CreateAnimationBlock(animation));
        }
    }
}
