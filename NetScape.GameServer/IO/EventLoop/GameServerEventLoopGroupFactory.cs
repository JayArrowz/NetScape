using NetScape.Abstractions.Interfaces.IO.EventLoop;
using DotNetty.Transport.Channels;

namespace NetScape.Modules.Server.IO.EventLoop
{
    public class GameServerEventLoopGroupFactory : BaseLoopGroupFactory, IEventLoopGroupFactory
    {
        public IEventLoopGroup GetOrCreateHandlerWorkerEventLoopGroup()
        {
            //TODO autofac config?
            return HandlerWorkerEventLoopGroup ?? (HandlerWorkerEventLoopGroup = NewEventLoopGroup(8));
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);
        }
    }

}
