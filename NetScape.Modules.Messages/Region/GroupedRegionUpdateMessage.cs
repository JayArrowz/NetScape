using DotNetty.Buffers;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Area;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;

namespace NetScape.Modules.Messages.Region
{
    public class GroupedRegionUpdateMessage : IOutMessage<MessageFrame>
    {

        /**
		 * The last known region Position of the Player.
		 */
        private readonly Position lastKnownRegion;

        /**
		 * The Set of RegionUpdateMessages to be sent.
		 */
        private readonly HashSet<RegionUpdateMessage> messages;

        /**
		 * The Position of the Region being updated.
		 */
        private readonly Position region;

        /**
		 * Creates the GroupedRegionUpdateMessage.
		 *
		 * @param lastKnownRegion The last known region {@link Position} of the Player.
		 * @param coordinates The {@link RegionCoordinates} of the Region being updated.
		 * @param messages The {@link Set} of {@link RegionUpdateMessage}s.
		 */
        public GroupedRegionUpdateMessage(Position lastKnownRegion, RegionCoordinates coordinates,
                                          HashSet<RegionUpdateMessage> messages)
        {
            this.lastKnownRegion = lastKnownRegion;
            region = new Position(coordinates.AbsoluteX, coordinates.AbsoluteY);
            this.messages = messages;
        }

        public MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            MessageFrameBuilder bldr = new MessageFrameBuilder(alloc, 60, FrameType.VariableShort);
            Position basePos = lastKnownRegion;

            bldr.Put(MessageType.Byte, region.GetLocalY(basePos));
            bldr.Put(MessageType.Byte, DataTransformation.Negate, region.GetLocalX(basePos));
            foreach (RegionUpdateMessage update in messages)
            {
                var frame = update.ToMessage(alloc);
                bldr.Put(MessageType.Byte, frame.Id);
                bldr.PutBytes(frame.Payload);
            }

            return bldr.ToMessageFrame();
        }
    }
}
