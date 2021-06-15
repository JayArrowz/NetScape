using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using NetScape.Abstractions.Interfaces.IO.EventLoop;

namespace NetScape.Modules.Server.EventLoop
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
