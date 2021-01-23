using DotNetty.Buffers;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Area.Update
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
        private readonly int orientation;

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
            orientation = obj.Orientation;
        }


        public override int Priority => Low_Priority;

        public override bool Equals(object obj)
        {
            if (obj is SendObjectMessage) {
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
            int result = prime * _id + orientation;
            result = prime * result + _type;
            return prime * result + _positionOffset;
        }

        public override MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            throw new NotImplementedException();
        }
    }
}
