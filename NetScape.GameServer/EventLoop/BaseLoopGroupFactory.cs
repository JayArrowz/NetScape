using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Modules.Server.EventLoop
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
        internal IEventLoopGroup WorkerEventLoopGroup;

        /// <summary>The socket io event loop group</summary>
        internal IEventLoopGroup BossEventLoopGroup;

        /// <summary>Initializes a new instance of the <see cref="BaseLoopGroupFactory"/> class.</summary>
        public BaseLoopGroupFactory()
        {
            _eventLoopGroupList = new List<IEventLoopGroup>();
        }

        /// <summary>Creates new Event Loop Group.</summary>
        /// <param name="nEventLoop">The number of event loops.</param>
        /// <returns></returns>
        internal IEventLoopGroup NewEventLoopGroup(int? nEventLoop = null)
        {
            var eventLoopGroup = CreateLoopGroup(nEventLoop);
            _eventLoopGroupList.Add(eventLoopGroup);
            return eventLoopGroup;
        }

        private MultithreadEventLoopGroup CreateLoopGroup(int? nEventLoop)
        {
            return nEventLoop.HasValue ? new MultithreadEventLoopGroup(nEventLoop.Value) : new MultithreadEventLoopGroup();
        }

        public IEventLoopGroup GetOrCreateSocketIoEventLoopGroup(int? nEventLoop = null)
        {
            return BossEventLoopGroup ?? (BossEventLoopGroup = NewEventLoopGroup(nEventLoop));
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
            WorkerEventLoopGroup = null;
            BossEventLoopGroup = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

}
