using DotNetty.Transport.Channels;
using System;

namespace NetScape.Abstractions.Interfaces.IO.EventLoop
{
    public interface IEventLoopGroupFactory : IDisposable
    {
        /// <summary>Gets or Creates new loop group for the netty handlers.</summary>
        IEventLoopGroup GetWorkerGroup();

        /// <summary>Gets or Creates new loop group for the socket IO.</summary>
        IEventLoopGroup GetBossGroup();
    }
}
