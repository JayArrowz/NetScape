using DotNetty.Buffers;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Modules.Messages.Region
{
    public class SendObjectMessage : RegionUpdateMessage
    {
        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public override int Priority => Low_Priority;

        /// <summary>
        /// The id of the object.
        /// </summary>
        private readonly int _id;

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


        /**
         * Creates the SendObjectMessage.
         *
         * @param object The {@link GameObject} to send.
         * @param positionOffset The offset of the object's position from the region's central position.
         */
        /// <summary>
        /// Initializes a new instance of the <see cref="SendObjectMessage"/> class.
        /// </summary>
        /// <param name="obj">The game object.</param>
        /// <param name="positionOffset">The position offset.</param>
        public SendObjectMessage(GameObject obj, int positionOffset)
        {
            _id = obj.Id;
            _positionOffset = positionOffset;
            _type = obj.Type;
            _orientation = obj.Orientation;
        }

        public override bool Equals(object obj)
        {
            if (obj is SendObjectMessage)
            {
                SendObjectMessage other = (SendObjectMessage)obj;
                if (_id != other._id || _type != other._type)
                {
                    return false;
                }

                return _positionOffset == other._positionOffset && _type == other._type;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = prime * _id + _orientation;
            result = prime * result + _type;
            return prime * result + _positionOffset;
        }

        public override MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            MessageFrameBuilder builder = new MessageFrameBuilder(alloc, 151);
            builder.Put(MessageType.Byte, DataTransformation.Add, _positionOffset);
            builder.Put(MessageType.Short, DataOrder.Little, _id);
            builder.Put(MessageType.Byte, DataTransformation.Subtract, _type << 2 | _orientation);
            return builder.ToMessageFrame();
        }
    }
}
