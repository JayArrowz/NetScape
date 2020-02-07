using ASPNetScape.Abstractions.Interfaces.IO;
using ASPNetScape.Abstractions.Interfaces.IO.EventLoop;
using ASPNetScape.Abstractions.IO;
using ASPNetScape.Abstractions.Model.IO;
using ASPNetScape.Modules.Server.IO;
using ASPNetScape.Modules.Server.IO.EventLoop;
using Autofac;

namespace ASPNetScape.Modules.Server
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
            builder.RegisterType<GameServerEventLoopGroupFactory>()
                .As<IEventLoopGroupFactory>();

            builder.RegisterInstance(_gameServerParams).As<IGameServerParameters>();
            base.Load(builder);
        }

    }
}
