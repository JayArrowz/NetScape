using DotNetty.Transport.Channels;
using System;

namespace ASPNetScape.Abstractions.Interfaces.IO.EventLoop
{
    public interface IEventLoopGroupFactory : IDisposable
    {
        /// <summary>Gets or Creates new loop group for the netty handlers.</summary>
        IEventLoopGroup GetOrCreateHandlerWorkerEventLoopGroup();

        /// <summary>Gets or Creates new loop group for the socket IO.</summary>
        IEventLoopGroup GetOrCreateSocketIoEventLoopGroup();
    }
}
