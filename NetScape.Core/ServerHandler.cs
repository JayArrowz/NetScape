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

namespace NetScape.Core
{
    public static class ServerHandler
    {
        public static ILifetimeScope RunServer(string configFileName, Action<DbContextOptionsBuilder, IConfigurationRoot> dbOptions, List<Module> modules)
        {
            var serviceCollection = new ServiceCollection();
            var config = ConfigureServices(serviceCollection, configFileName, dbOptions);
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            ConfigureAutofac(containerBuilder, config, modules);
            containerBuilder.RegisterBuildCallback(t => t.Resolve<ContainerProvider>().Container = (IContainer)t);
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            ILifetimeScope scope = container.BeginLifetimeScope();
            var gameServer = serviceProvider.GetRequiredService<IGameServer>();
            _ = gameServer.BindAsync();
            return scope;
        }

        public static void ConfigureCore(this ContainerBuilder containerBuilder, IConfigurationRoot configurationRoot)
        {
            containerBuilder.RegisterModule(new SeriLogModule(configurationRoot));
            containerBuilder.RegisterModule(new CacheModule());
            containerBuilder.RegisterModule(new DALModule());
            containerBuilder.RegisterModule(new GameServerModule(configurationRoot["BindAddr"], ushort.Parse(configurationRoot["BindPort"])));
            containerBuilder.RegisterModule(new WorldModule());
            containerBuilder.RegisterModule(new RegionModule());
            containerBuilder.RegisterModule(new CollisionModule());
            containerBuilder.RegisterType<WalkingQueueHandler>();
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
            containerBuilder.RegisterType<ContainerProvider>().SingleInstance();
        }

        private static void ConfigureAutofac(ContainerBuilder containerBuilder, IConfigurationRoot configurationRoot, List<Module> modules)
        {
            foreach (var module in modules)
            {
                containerBuilder.RegisterModule(module);
            }

            containerBuilder.ConfigureCore(configurationRoot);
        }

        public static IConfigurationRoot ConfigureServices(this IServiceCollection serviceCollection, string configFileName, Action<DbContextOptionsBuilder, IConfigurationRoot> optionsAction)
        {
            var configurationRoot = CreateConfigurationRoot(configFileName);

            // Add logging
            serviceCollection.AddLogging();

            //Build DB Connection
            serviceCollection.AddDbContextFactory<DatabaseContext>(opts => optionsAction(opts, configurationRoot));

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
