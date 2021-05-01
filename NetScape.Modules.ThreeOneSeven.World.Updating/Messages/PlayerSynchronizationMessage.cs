using DotNetty.Buffers;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.World.Updating.Blocks;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using NetScape.Modules.World.Updating.Segments;
using System.Collections.Generic;

namespace NetScape.Modules.World.Updating
{
    public class PlayerSynchronizationMessage : IEncoderMessage<MessageFrame>
    {
        public Position LastKnownRegion { get; }
        public int LocalPlayers { get; }
        public Position Position { get; }
        public bool RegionChanged { get; }
        public SynchronizationSegment Segment { get; }
        public List<SynchronizationSegment> Segments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSynchronizationMessage"/> class.
        /// </summary>
        /// <param name="lastKnownRegion">The last known region.</param>
        /// <param name="position">The players current position.</param>
        /// <param name="regionChanged">if set to <c>true</c> [region changed].</param>
        /// <param name="segment">The current player's synchronization segment.</param>
        /// <param name="localPlayers">The number local players.</param>
        /// <param name="segments">The list of segments.</param>
        public PlayerSynchronizationMessage(Position lastKnownRegion, Position position, bool regionChanged, SynchronizationSegment segment, int localPlayers, List<SynchronizationSegment> segments)
        {
            LastKnownRegion = lastKnownRegion;
            Position = position;
            RegionChanged = regionChanged;
            Segment = segment;
            LocalPlayers = localPlayers;
            Segments = segments;
        }


        public MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            var bldr = new MessageFrameBuilder(alloc, 81, FrameType.VariableShort);
            bldr.SwitchToBitAccess();

            var blockBuilder = new MessageFrameBuilder(alloc);
            PutMovementUpdate(Segment, bldr);
            PutBlocks(Segment, blockBuilder);
            bldr.PutBits(8, LocalPlayers);

            foreach (SynchronizationSegment segment in Segments)
            {
                SegmentType type = segment.Type;
                if (type == SegmentType.Remove_Mob)
                {
                    PutRemovePlayerUpdate(bldr);
                }
                else if (type == SegmentType.Add_Mob)
                {
                    PutAddPlayerUpdate((AddPlayerSegment)segment, bldr);
                    PutBlocks(segment, blockBuilder);
                }
                else
                {
                    PutMovementUpdate(segment, bldr);
                    PutBlocks(segment, blockBuilder);
                }
            }

            if (blockBuilder.GetLength() > 0)
            {
                bldr.PutBits(11, 2047);
                bldr.SwitchToByteAccess();
                bldr.PutRawBuilder(blockBuilder);
            }
            else
            {
                bldr.SwitchToByteAccess();
            }

            return bldr.ToMessageFrame();

        }

        private void PutAddPlayerUpdate(AddPlayerSegment seg, MessageFrameBuilder builder)
        {
            var updateRequired = seg.BlockSet.Size() > 0;
            Position player = Position;
            Position other = seg.Position;
            builder.PutBits(11, seg.Index);
            builder.PutBits(1, updateRequired ? 1 : 0);
            builder.PutBits(1, 1); // discard walking queue?
            builder.PutBits(5, other.Y - player.Y);
            builder.PutBits(5, other.X - player.X);
        }

        private void PutRemovePlayerUpdate(MessageFrameBuilder builder)
        {
            builder.PutBits(1, 1);
            builder.PutBits(2, 3);
        }

        private void PutBlocks(SynchronizationSegment segment, MessageFrameBuilder builder)
        {
            var blockSet = segment.BlockSet;
            if (blockSet.Size() > 0)
            {
                int mask = 0;
                if (blockSet.Contains<AnimationBlock>())
                {
                    mask |= 0x8;
                }

                if (blockSet.Contains<AppearanceBlock>())
                {
                    mask |= 0x10;
                }

                if (mask >= 0x100)
                {
                    mask |= 0x40;
                    builder.Put(MessageType.Short, DataOrder.Little, mask);
                }
                else
                {
                    builder.Put(MessageType.Byte, mask);
                }

                if (blockSet.Contains<AnimationBlock>())
                {
                    PutAnimationBlock(blockSet.Get<AnimationBlock>(), builder);
                }

                if (blockSet.Contains<AppearanceBlock>())
                {
                    PutAppearanceBlock(blockSet.Get<AppearanceBlock>(), builder);
                }
            }
        }

        private void PutAppearanceBlock(AppearanceBlock block, MessageFrameBuilder builder)
        {
            Appearance appearance = block.Appearance;
            var playerProperties = new MessageFrameBuilder(builder.Alloc);

            playerProperties.Put(MessageType.Byte, (int)appearance.Gender);
            playerProperties.Put(MessageType.Byte, 0);

            if (block.NpcId > 0)
            {
                playerProperties.Put(MessageType.Byte, 255);
                playerProperties.Put(MessageType.Byte, 255);
                playerProperties.Put(MessageType.Short, block.NpcId);
            }
            else
            {
                //Inventory equipment = block.getEquipment();
                int[] style = appearance.Style;
                //Item item, chest, helm;

                for (int slot = 0; slot < 4; slot++)
                {
                    playerProperties.Put(MessageType.Byte, 0);
                }

                playerProperties.Put(MessageType.Short, 0x100 + style[2]);
                playerProperties.Put(MessageType.Byte, 0);
                playerProperties.Put(MessageType.Short, 0x100 + style[3]);
                playerProperties.Put(MessageType.Short, 0x100 + style[5]);
                playerProperties.Put(MessageType.Short, 0x100 + style[0]);
                playerProperties.Put(MessageType.Short, 0x100 + style[4]);
                playerProperties.Put(MessageType.Short, 0x100 + style[6]);
                playerProperties.Put(MessageType.Short, 0x100 + style[1]);
            }

            int[] colors = appearance.Colors;
            foreach (int color in colors)
            {
                playerProperties.Put(MessageType.Byte, color);
            }

            playerProperties.Put(MessageType.Short, 0x328); // stand
            playerProperties.Put(MessageType.Short, 0x337); // stand turn
            playerProperties.Put(MessageType.Short, 0x333); // walk
            playerProperties.Put(MessageType.Short, 0x334); // turn 180
            playerProperties.Put(MessageType.Short, 0x335); // turn 90 cw
            playerProperties.Put(MessageType.Short, 0x336); // turn 90 ccw
            playerProperties.Put(MessageType.Short, 0x338); // run

            playerProperties.Put(MessageType.Long, block.Name);
            playerProperties.Put(MessageType.Byte, block.Combat);
            playerProperties.Put(MessageType.Short, block.Skill);

            builder.Put(MessageType.Byte, DataTransformation.Negate, playerProperties.GetLength());

            builder.PutRawBuilder(playerProperties);
        }

        private static void PutAnimationBlock(AnimationBlock block, MessageFrameBuilder builder)
        {
            var animation = block.Animation;
            builder.Put(MessageType.Short, DataOrder.Little, animation.Id);
            builder.Put(MessageType.Byte, DataTransformation.Negate, animation.Delay);

        }

        private void PutMovementUpdate(SynchronizationSegment seg, MessageFrameBuilder builder)
        {
            bool updateRequired = seg.BlockSet.Size() > 0;
            if (seg.Type == SegmentType.Teleport)
            {
                var teleportSeg = ((TeleportSegment)seg);
                Position position = teleportSeg.Destination;
                builder.PutBits(1, 1);
                builder.PutBits(2, 3);
                builder.PutBits(2, position.Height);
                builder.PutBits(1, RegionChanged ? 0 : 1);
                builder.PutBits(1, updateRequired ? 1 : 0);
                builder.PutBits(7, position.GetLocalY(LastKnownRegion));
                builder.PutBits(7, position.GetLocalX(LastKnownRegion));
            }
            else if (seg.Type == SegmentType.Run)
            {
                Direction[] directions = ((MovementSegment)seg).Directions;
                builder.PutBits(1, 1);
                builder.PutBits(2, 2);
                builder.PutBits(3, directions[0].IntValue);
                builder.PutBits(3, directions[1].IntValue);
                builder.PutBits(1, updateRequired ? 1 : 0);
            }
            else if (seg.Type == SegmentType.Walk)
            {
                Direction[] directions = ((MovementSegment)seg).Directions;
                builder.PutBits(1, 1);
                builder.PutBits(2, 1);
                builder.PutBits(3, directions[0].IntValue);
                builder.PutBits(1, updateRequired ? 1 : 0);
            }
            else
            {
                if (updateRequired)
                {
                    builder.PutBits(1, 1);
                    builder.PutBits(2, 0);
                }
                else
                {
                    builder.PutBits(1, 0);
                }
            }
        }

    }
}
