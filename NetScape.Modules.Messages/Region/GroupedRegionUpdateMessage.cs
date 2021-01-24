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
        /// <summary>
        /// The last known region of the player
        /// </summary>
        private readonly Position _lastKnownRegion;

        /// <summary>
        /// The set of the messages to be sent
        /// </summary>
        private readonly HashSet<RegionUpdateMessage> _messages;

        /// <summary>
        /// The position of the region being updated
        /// </summary>
        private readonly Position _update;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedRegionUpdateMessage"/> class.
        /// </summary>
        /// <param name="lastKnownRegion">The last known region.</param>
        /// <param name="coordinates">The coordinates of the region being updated.</param>
        /// <param name="messages">The set of the messages to be sent.</param>
        public GroupedRegionUpdateMessage(Position lastKnownRegion, RegionCoordinates coordinates,
                                          HashSet<RegionUpdateMessage> messages)
        {
            _lastKnownRegion = lastKnownRegion;
            _update = new Position(coordinates.AbsoluteX, coordinates.AbsoluteY);
            _messages = messages;
        }

        public MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            MessageFrameBuilder bldr = new MessageFrameBuilder(alloc, 60, FrameType.VariableShort);
            Position basePos = _lastKnownRegion;

            bldr.Put(MessageType.Byte, _update.GetLocalY(basePos));
            bldr.Put(MessageType.Byte, DataTransformation.Negate, _update.GetLocalX(basePos));
            foreach (RegionUpdateMessage update in _messages)
            {
                var frame = update.ToMessage(alloc);
                bldr.Put(MessageType.Byte, frame.Id);
                bldr.PutBytes(frame.Payload);
            }

            return bldr.ToMessageFrame();
        }
    }
}
