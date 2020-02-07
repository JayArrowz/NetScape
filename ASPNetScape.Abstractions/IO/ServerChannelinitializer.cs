using ASPNetScape.Abstractions.Interfaces.IO.Login;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Abstractions.IO
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
            pipeline.AddLast(_loginProvider.Handlers());
        }
    }
}
