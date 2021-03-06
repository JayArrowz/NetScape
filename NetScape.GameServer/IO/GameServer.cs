﻿using NetScape.Abstractions.Interfaces.IO;
using NetScape.Abstractions.Interfaces.IO.EventLoop;
using NetScape.Abstractions.IO;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Serilog;
using System.Net;
using System.Threading.Tasks;

namespace NetScape.Modules.Server.IO
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
            var bossGroup = _eventLoopGroupFactory.GetBossGroup();
            var workerGroup = _eventLoopGroupFactory.GetWorkerGroup();
            var bootstrap = new ServerBootstrap();

            bootstrap.ChannelFactory(() => new TcpServerSocketChannel());
            bootstrap.Group(bossGroup, workerGroup);
            bootstrap.ChildHandler(_serverChannelInitializer);

            Channel = await bootstrap.BindAsync(IPAddress.Parse(_gameServerParameters.BindAddress), _gameServerParameters.Port);
            _logger.Information("Network Bound NIC IP: {0} Port: {1}", _gameServerParameters.BindAddress, _gameServerParameters.Port);
        }

        public async ValueTask DisposeAsync()
        {
            await Channel.DisconnectAsync();
            _eventLoopGroupFactory.Dispose();
        }
    }
}
