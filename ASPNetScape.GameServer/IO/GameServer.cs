using ASPNetScape.Abstractions.Interfaces.IO;
using ASPNetScape.Abstractions.Interfaces.IO.EventLoop;
using ASPNetScape.Abstractions.IO;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Serilog;
using System.Net;
using System.Threading.Tasks;

namespace ASPNetScape.Modules.Server.IO
{
    public sealed class GameServer : IGameServer
    {
        private readonly IEventLoopGroupFactory _eventLoopGroupFactory;
        private readonly IGameServerParameters _gameServerParameters;
        private readonly ILogger _logger;
        private readonly ServerChannelInitializer _serverChannelInitializer;

        public GameServer(ILogger logger,
            IGameServerParameters gameServerParameters,
            IEventLoopGroupFactory eventLoopGroupFactory,
            ServerChannelInitializer serverChannelInitializer)
        {
            _logger = logger;
            _gameServerParameters = gameServerParameters;
            _eventLoopGroupFactory = eventLoopGroupFactory;
            _serverChannelInitializer = serverChannelInitializer;
        }

        public IChannel Channel { get; set; }

        public async Task BindAsync()
        {
            var bossGroup = _eventLoopGroupFactory.GetOrCreateSocketIoEventLoopGroup();
            var workerGroup = _eventLoopGroupFactory.GetOrCreateHandlerWorkerEventLoopGroup();
            var bootstrap = new ServerBootstrap();

            bootstrap.ChannelFactory(() => new TcpServerSocketChannel());
            bootstrap.Group(bossGroup, workerGroup);
            bootstrap.ChildHandler(_serverChannelInitializer);

            Channel = await bootstrap.BindAsync(IPAddress.Parse(_gameServerParameters.BindAddress), _gameServerParameters.Port);
            _logger.Information($"Network Bound NIC IP: {_gameServerParameters.BindAddress} Port: {_gameServerParameters.Port}");
        }

        public void Dispose()
        {
            _ = Channel.DisconnectAsync();
        }
    }
}
