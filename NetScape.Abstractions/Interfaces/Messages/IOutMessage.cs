using DotNetty.Buffers;

namespace NetScape.Modules.Messages
{
    public interface IOutMessage<T>
    {
        T ToMessage(IByteBufferAllocator alloc);
    }
}
