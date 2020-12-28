using DotNetty.Buffers;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Abstractions.Model.World.Updating.Blocks;
using NetScape.Modules.Messages.Builder;
using NetScape.Modules.World.Updating.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.Messages.Outgoing
{
    public class PlayerSynchronizationMessage
    {
        public Position LastKnownRegion { get; set; }
        public int LocalPlayers { get; set; }
        public Position Position { get; set; }
        public bool RegionChanged { get; set; }
        public SynchronizationSegment Segment { get; set; }
        public List<SynchronizationSegment> Segments { get; set; }

        public MessageFrame ToMessageFrame(IByteBufferAllocator alloc)
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

        private static void PutRemovePlayerUpdate(MessageFrameBuilder builder)
        {
            builder.PutBits(1, 1);
            builder.PutBits(2, 3);
        }

        private static void PutBlocks(SynchronizationSegment segment, MessageFrameBuilder builder)
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

        private static void PutAppearanceBlock(AppearanceBlock block, MessageFrameBuilder builder)
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

        private static void PutMovementUpdate(SynchronizationSegment seg, MessageFrameBuilder builder)
        {
            bool updateRequired = seg.BlockSet.Size() > 0;
            if (seg.Type == SegmentType.Teleport)
            {
                /*Position position = ((TeleportSegment)seg).getDestination();
				builder.PutBits(1, 1);
				builder.PutBits(2, 3);
				builder.PutBits(2, position.getHeight());
				builder.PutBits(1, message.hasRegionChanged() ? 0 : 1);
				builder.PutBits(1, updateRequired ? 1 : 0);
				builder.PutBits(7, position.getLocalY(message.getLastKnownRegion()));
				builder.PutBits(7, position.getLocalX(message.getLastKnownRegion()));*/
            }
            else if (seg.Type == SegmentType.Run)
            {
                /*Direction[] directions = ((MovementSegment)seg).getDirections();
				builder.PutBits(1, 1);
				builder.PutBits(2, 2);
				builder.PutBits(3, directions[0].toInteger());
				builder.PutBits(3, directions[1].toInteger());
				builder.PutBits(1, updateRequired ? 1 : 0);*/
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
