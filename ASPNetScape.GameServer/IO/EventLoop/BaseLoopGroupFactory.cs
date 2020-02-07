using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Modules.Server.IO.EventLoop
{
    /// <summary>
    /// The <see cref="BaseLoopGroupFactory"/> class keeps references of event loop groups that are created
    /// and holds them from disposing
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class BaseLoopGroupFactory : IDisposable
    {
        /// <summary>The quiet period for the event loop group before shutdown</summary>
        private readonly long QuietPeriod = 100;

        private int _disposeCounter;

        /// <summary>The event loop group list</summary>
        private readonly List<IEventLoopGroup> _eventLoopGroupList;

        /// <summary>The handler worker event loop group</summary>
        internal IEventLoopGroup HandlerWorkerEventLoopGroup;

        /// <summary>The socket io event loop group</summary>
        internal IEventLoopGroup SocketIoEventLoopGroup;

        /// <summary>Initializes a new instance of the <see cref="BaseLoopGroupFactory"/> class.</summary>
        public BaseLoopGroupFactory()
        {
            _eventLoopGroupList = new List<IEventLoopGroup>();
        }

        /// <summary>Creates new Event Loop Group.</summary>
        /// <param name="nEventLoop">The number of event loops.</param>
        /// <returns></returns>
        internal IEventLoopGroup NewEventLoopGroup(int nEventLoop)
        {
            var eventLoopGroup = new MultithreadEventLoopGroup(nEventLoop);
            _eventLoopGroupList.Add(eventLoopGroup);
            return eventLoopGroup;
        }

        /// <summary>Creates new event loop group with the default amount of event loops <see cref="DotNetty.Transport.Channels.MultithreadEventLoopGroup"/>.</summary>
        /// <returns></returns>
        internal IEventLoopGroup NewEventLoopGroup()
        {
            var eventLoopGroup = new MultithreadEventLoopGroup();
            _eventLoopGroupList.Add(eventLoopGroup);
            return eventLoopGroup;
        }

        public IEventLoopGroup GetOrCreateSocketIoEventLoopGroup()
        {
            return SocketIoEventLoopGroup ?? (SocketIoEventLoopGroup = NewEventLoopGroup());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Interlocked.Increment(ref _disposeCounter) > 1)
            {
                return;
            }

            Task[] disposeTasks = _eventLoopGroupList.Select(t =>
                    t.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(QuietPeriod), TimeSpan.FromMilliseconds(QuietPeriod * 3)))
               .ToArray();

            Task.WaitAll(disposeTasks, TimeSpan.FromMilliseconds(QuietPeriod * 4 * disposeTasks.Length));
            _eventLoopGroupList.Clear();
            HandlerWorkerEventLoopGroup = null;
            SocketIoEventLoopGroup = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

}
