using DotNetty.Transport.Channels;
using System;

namespace NetScape.Abstractions.Interfaces.IO.EventLoop
{
    /// <summary>
    /// Handles distribution and disposal of all event loops
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEventLoopGroupFactory : IDisposable
    {
        /// <summary>Gets or Creates new loop group for the netty handlers.</summary>
        IEventLoopGroup GetWorkerGroup();

        /// <summary>Gets or Creates new loop group for the socket IO.</summary>
        IEventLoopGroup GetBossGroup();
    }
}
