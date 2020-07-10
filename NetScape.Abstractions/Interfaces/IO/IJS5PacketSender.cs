using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace NetScape.Abstractions.Interfaces.IO
{
    public interface IJS5PacketSender
    {
        void SendStartup(IChannelHandlerContext ctx);
        IByteBuffer CreateUserKeysBuffer();
        void SendCacheArchive(IChannelHandlerContext channel, int indexId, int archiveId, bool priority);

        IByteBuffer GetContainerPacketData(int indexFileId, int containerId, IByteBuffer archive);
    }
}