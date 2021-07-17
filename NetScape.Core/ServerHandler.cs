using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Game;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Abstractions.Server;
using NetScape.Modules.Cache;
using NetScape.Modules.DAL;
using NetScape.Modules.Logging.SeriLog;
using NetScape.Modules.Region;
using NetScape.Modules.Region.Collision;
using NetScape.Modules.Server;
using NetScape.Modules.World;
using System;
using System.Collections.Generic;
using System.IO;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Core
{
    public static class ServerHandler
    {
        public static ILifetimeScope RunServer<TPlayer>(string configFileName, Action<DbContextOptionsBuilder, IConfigurationRoot> dbOptions, List<Module> modules)
            where TPlayer : Player, new()
        {
            var serviceCollection = new ServiceCollection();
            var config = ConfigureServices<TPlayer>(serviceCollection, configFileName, dbOptions);
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            ConfigureAutofac<TPlayer>(containerBuilder, config, modules);
            containerBuilder.RegisterBuildCallback(t => t.Resolve<ContainerProvider>().Container = (IContainer)t);
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            ILifetimeScope scope = container.BeginLifetimeScope();
            var gameServer = serviceProvider.GetRequiredService<IGameServer>();
            _ = gameServer.BindAsync();
            return scope;
        }

        public static void ConfigureCore<TPlayer>(this ContainerBuilder containerBuilder, IConfigurationRoot configurationRoot)
            where TPlayer : Player, new()
        {
            containerBuilder.RegisterModule(new SeriLogModule(configurationRoot));
            containerBuilder.RegisterModule(new CacheModule());
            containerBuilder.RegisterModule(new DALModule<TPlayer>());
            containerBuilder.RegisterModule(new GameServerModule(configurationRoot["BindAddr"], ushort.Parse(configurationRoot["BindPort"])));
            containerBuilder.RegisterModule(new WorldModule());
            containerBuilder.RegisterModule(new RegionModule());
            containerBuilder.RegisterModule(new CollisionModule());
            containerBuilder.RegisterType<WalkingQueueHandler>();
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
            containerBuilder.RegisterType<ContainerProvider>().SingleInstance();
        }

        private static void ConfigureAutofac<TPlayer>(ContainerBuilder containerBuilder, IConfigurationRoot configurationRoot, List<Module> modules)
            where TPlayer : Player, new()
        {
            foreach (var module in modules)
            {
                containerBuilder.RegisterModule(module);
            }

            containerBuilder.ConfigureCore<TPlayer>(configurationRoot);
        }

        public static IConfigurationRoot ConfigureServices<TPlayer>(this IServiceCollection serviceCollection, string configFileName, Action<DbContextOptionsBuilder, IConfigurationRoot> optionsAction)
            where TPlayer : Player, new()
        {
            var configurationRoot = CreateConfigurationRoot(configFileName);

            // Add logging
            serviceCollection.AddLogging();

            //Build DB Connection
            serviceCollection.AddDbContextFactory<DatabaseContext<TPlayer>>(opts => optionsAction(opts, configurationRoot));

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(configurationRoot);
            return configurationRoot;
        }

        public static IConfigurationRoot CreateConfigurationRoot(string fileName)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile(fileName, false)
                .Build();
        }
    }
}
