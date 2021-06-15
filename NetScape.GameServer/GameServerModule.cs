using Autofac;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Abstractions.Interfaces.IO.EventLoop;
using NetScape.Abstractions.IO;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Server.EventLoop;

namespace NetScape.Modules.Server
{
    public sealed class GameServerModule : Module
    {
        private readonly IGameServerParameters _gameServerParams;

        public GameServerModule(string bindAddr, ushort port)
        {
            _gameServerParams =
                new GameServerParameters { BindAddress = bindAddr, Port = port };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GameServer>().As<IGameServer>();
            builder.RegisterType<ServerChannelInitializer>();
            builder.RegisterType<GameServerChannelHandler>().AsSelf();
            builder.RegisterType<GameServerEventLoopGroupFactory>()
                .As<IEventLoopGroupFactory>();
            builder.RegisterInstance(_gameServerParams).As<IGameServerParameters>();
            base.Load(builder);
        }

    }
}
