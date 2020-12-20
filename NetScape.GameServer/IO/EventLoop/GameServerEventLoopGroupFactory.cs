using NetScape.Abstractions.Interfaces.IO.EventLoop;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace NetScape.Modules.Server.IO.EventLoop
{
    public class GameServerEventLoopGroupFactory : BaseLoopGroupFactory, IEventLoopGroupFactory
    {
        public IEventLoopGroup GetBossGroup()
        {
            return BossEventLoopGroup ?? (BossEventLoopGroup = NewEventLoopGroup(1));
        }

        public IEventLoopGroup GetWorkerGroup()
        {
            //TODO autofac config?
            return WorkerEventLoopGroup ?? (WorkerEventLoopGroup = NewEventLoopGroup(8));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);
        }
    }

}
