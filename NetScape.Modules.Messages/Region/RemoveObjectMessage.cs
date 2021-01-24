using DotNetty.Buffers;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Modules.Messages.Region
{
    public class RemoveObjectMessage : RegionUpdateMessage
    {
        /// <summary>
        /// The orientation of the object.
        /// </summary>
        private readonly int _orientation;

        /// <summary>
        /// The position of the object.
        /// </summary>
        private readonly int _positionOffset;

        /// <summary>
        /// The type of the object.
        /// </summary>
        private readonly int _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveObjectMessage"/> class.
        /// </summary>
        /// <param name="obj">The <see cref="GameObject"/> to send.</param>
        /// <param name="positionOffset">The offset of the object's position from the region's central position.</param>
        public RemoveObjectMessage(GameObject obj, int positionOffset)
        {
            _positionOffset = positionOffset;
            _type = obj.Type;
            _orientation = obj.Orientation;
        }

        public override int Priority => Low_Priority;

        public override bool Equals(object obj)
        {
            if (obj is RemoveObjectMessage)
            {
                RemoveObjectMessage other = (RemoveObjectMessage)obj;
                return _type == other._type && _orientation == other._orientation && _positionOffset == other._positionOffset;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = prime * _positionOffset + _orientation;
            return prime * result + _type;
        }

        public override MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            var bldr = new MessageFrameBuilder(alloc, 101);
            bldr.Put(MessageType.Byte, DataTransformation.Negate, _type << 2 | _orientation);
            bldr.Put(MessageType.Byte, _positionOffset);
            return bldr.ToMessageFrame();
        }
    }
}
