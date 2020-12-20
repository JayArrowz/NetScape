using NetScape.Abstractions.Interfaces.IO.EventLoop;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NetScape.Modules.Server.IO.EventLoop
{
    public class GameServerEventLoopGroupFactory : BaseLoopGroupFactory, IEventLoopGroupFactory
    {
        private const string BossGroupThreadCountConfigKey = "BossGroupThreadCount";
        private const string WorkerGroupThreadCountConfigKey = "WorkerGroupThreadCount";
        private readonly IConfigurationRoot _configurationRoot;

        public GameServerEventLoopGroupFactory(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public IEventLoopGroup GetBossGroup()
        {
            return BossEventLoopGroup ?? (BossEventLoopGroup = NewEventLoopGroup(int.Parse(_configurationRoot[BossGroupThreadCountConfigKey])));
        }

        public IEventLoopGroup GetWorkerGroup()
        {
            return WorkerEventLoopGroup ?? (WorkerEventLoopGroup = NewEventLoopGroup(int.Parse(_configurationRoot[WorkerGroupThreadCountConfigKey])));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);
        }
    }

}
