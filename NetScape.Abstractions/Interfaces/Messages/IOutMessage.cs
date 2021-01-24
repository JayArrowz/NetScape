using DotNetty.Buffers;

namespace NetScape.Modules.Messages
{
    /// <summary>
    /// Outgoing message encoder
    /// </summary>
    /// <typeparam name="T">The type of message</typeparam>
    public interface IOutMessage<T>
    {
        /// <summary>
        /// Converts to message.
        /// </summary>
        /// <param name="alloc">The allocator.</param>
        /// <returns>T</returns>
        T ToMessage(IByteBufferAllocator alloc);
    }
}
