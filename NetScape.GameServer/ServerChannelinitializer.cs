using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Login;

namespace NetScape.Modules.Server
{
    public class ServerChannelInitializer : ChannelInitializer<IChannel>
    {
        private readonly ILoginProvider _loginProvider;

        public ServerChannelInitializer(ILoginProvider loginProvider)
        {
            _loginProvider = loginProvider;
        }

        protected override void InitChannel(IChannel channel)
        {
            var pipeline = channel.Pipeline;
            pipeline.AddLast(new GameServerChannelHandler());
            pipeline.AddLast(_loginProvider.Provide());
        }
    }
}
