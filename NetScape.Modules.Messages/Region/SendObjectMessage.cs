using DotNetty.Buffers;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Modules.Messages.Region
{
    public class SendObjectMessage : RegionUpdateMessage
    {
        /**
	     * The id of the object.
	     */
        private readonly int _id;

        /**
         * The orientation of the object.
         */
        private readonly int _orientation;

        /**
         * The position of the object.
         */
        private readonly int _positionOffset;

        /**
         * The type of the object.
         */
        private readonly int _type;


        /**
         * Creates the SendObjectMessage.
         *
         * @param object The {@link GameObject} to send.
         * @param positionOffset The offset of the object's position from the region's central position.
         */
        public SendObjectMessage(GameObject obj, int positionOffset)
        {
            _id = obj.Id;
            _positionOffset = positionOffset;
            _type = obj.Type;
            _orientation = obj.Orientation;
        }


        public override int Priority => Low_Priority;

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
