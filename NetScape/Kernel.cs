using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Modules.Logging.SeriLog;
using NetScape.Modules.Server;
using NetScape.Modules.SevenOneEight.Cache;
using NetScape.Modules.ThreeOneSeven.LoginProtocol;
using Autofac;
using System;

namespace NetScape
{
    public class Kernel
    {
        public static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new SeriLogModule());
            containerBuilder.RegisterModule(new ThreeOneSevenLoginModule());
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
