using ASPNetScape.Abstractions.FileSystem;
using ASPNetScape.Abstractions.Interfaces.IO;
using ASPNetScape.Modules.Logging.SeriLog;
using ASPNetScape.Modules.Server;
using ASPNetScape.Modules.SevenOneEight.Cache;
using ASPNetScape.Modules.SevenOneEight.LoginProtocol;
using Autofac;
using System;

namespace ASPNetScape
{
    public class Kernel
    {
        public static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new SeriLogModule());
            containerBuilder.RegisterModule(new SevenOneEightLoginModule());
            containerBuilder.RegisterModule(new SevenOneEightCacheModule());
            containerBuilder.RegisterModule(new GameServerModule("127.0.0.1", 43594));
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();

            var container = containerBuilder.Build();

            var gameServer = container.Resolve<IGameServer>();
            _ = gameServer.BindAsync();

            //TODO: Better way
            Console.ReadLine();
        }
    }
}
