using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Abstractions.Interfaces.IO.Messaging
{
    public interface IIncomingPacket<T>
    {
        Task<T> ProcessMessage(IChannelHandlerContext context, IByteBuffer buffer);

        int PacketId { get; }
    }
}
