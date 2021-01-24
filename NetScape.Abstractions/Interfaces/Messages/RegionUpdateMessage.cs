using DotNetty.Buffers;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public abstract class RegionUpdateMessage : IOutMessage<MessageFrame>
    {
        /// <summary>
        /// The integer value indicating this RegionUpdateMessage is a high-priority message.
        /// </summary>
        protected static readonly int High_Priority = 0;

        /// <summary>
        /// The integer value indicating this RegionUpdateMessage is a low-priority message.
        /// </summary>
        protected static readonly int Low_Priority = 1;

        /// <summary>
        /// Converts to message.
        /// </summary>
        /// <param name="alloc">The bytebuffer allocator.</param>
        /// <returns></returns>
        public abstract MessageFrame ToMessage(IByteBufferAllocator alloc);

        /// <summary>
        /// Gets the priority. <see cref="High_Priority"/> <seealso cref="Low_Priority"/>
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public abstract int Priority { get; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public abstract override bool Equals(object o);
    }
}
