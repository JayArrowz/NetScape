using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Modules.Logging.SeriLog;
using NetScape.Modules.Server;
using Autofac;
using System;
using NetScape.Modules.SevenOneEight.Cache;
using System.Threading.Tasks;
using NetScape.Modules.LoginProtocol;
using NetScape.Abstractions.Interfaces;

namespace NetScape
{
    public class Kernel
    {
        public static async Task Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new SeriLogModule());
            containerBuilder.RegisterModule(new LoginModule());
            containerBuilder.RegisterModule(new CacheModule());
            containerBuilder.RegisterModule(new GameServerModule("127.0.0.1", 43594));
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
            containerBuilder.RegisterType<ContainerProvider>().SingleInstance();

            var container = containerBuilder.Build();
            container.Resolve<ContainerProvider>().Container = container;

            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                var gameServer = container.Resolve<IGameServer>();
                _ = gameServer.BindAsync();
                Console.ReadLine();
            }
        }
    }
}
