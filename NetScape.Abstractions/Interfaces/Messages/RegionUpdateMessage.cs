using DotNetty.Buffers;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public abstract class RegionUpdateMessage : IOutMessage<MessageFrame>
    {
        /**
	     * The integer value indicating this RegionUpdateMessage is a high-priority message.
	     */
        protected static readonly int High_Priority = 0;

        /**
         * The integer value indicating this RegionUpdateMessage is a low-priority message.
         */
        protected static readonly int Low_Priority = 1;

        public abstract MessageFrame ToMessage(IByteBufferAllocator alloc);

        public abstract int Priority { get; }

        public abstract override int GetHashCode();
        public abstract override bool Equals(object o);
    }
}
