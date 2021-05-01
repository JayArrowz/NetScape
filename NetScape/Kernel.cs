using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Abstractions.Model.Game.Walking;
using NetScape.Modules.Cache;
using NetScape.Modules.DAL;
using NetScape.Modules.Game;
using NetScape.Modules.Logging.SeriLog;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Models;
using NetScape.Modules.Region;
using NetScape.Modules.Region.Collision;
using NetScape.Modules.Server;
using NetScape.Modules.ThreeOneSeven.LoginProtocol;
using NetScape.Modules.World;
using NetScape.Modules.World.Updating;
using System;
using System.IO;

namespace NetScape
{
    public class Kernel
    {
        public static IConfigurationRoot ConfigurationRoot { get; set; }
        public static bool Exited { get; set; }
        public static void Main()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            ConfigureAutofac(containerBuilder);
            containerBuilder.RegisterBuildCallback(t => t.Resolve<ContainerProvider>().Container = (IContainer)t);
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                var gameServer = serviceProvider.GetRequiredService<IGameServer>();
                _ = gameServer.BindAsync();

                //TODO Make better
                Console.ReadLine();
                Exited = true;
            }
        }

        private static void BuildDbOptions(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConfigurationRoot.GetConnectionString("NetScape"),
                 x => x.MigrationsAssembly(typeof(DatabaseContext)
                    .Assembly.GetName().Name));
        }

        private static void ConfigureAutofac(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new ThreeOneSevenGameModule());
            containerBuilder.RegisterModule(new MessagesModule(
                typeof(ThreeOneSevenEncoderMessages.Types),
                typeof(ThreeOneSevenDecoderMessages.Types))
            );
            containerBuilder.RegisterModule(new ThreeOneSevenLoginModule());
            containerBuilder.RegisterModule(new ThreeOneSevenUpdatingModule());

            containerBuilder.RegisterModule(new SeriLogModule(ConfigurationRoot));
            containerBuilder.RegisterModule(new CacheModule());
            containerBuilder.RegisterModule(new DALModule());           
            containerBuilder.RegisterModule(new GameServerModule(ConfigurationRoot["BindAddr"], ushort.Parse(ConfigurationRoot["BindPort"])));
            containerBuilder.RegisterModule(new WorldModule());
            containerBuilder.RegisterModule(new RegionModule());
            containerBuilder.RegisterModule(new CollisionModule());
            containerBuilder.RegisterType<WalkingQueueHandler>();
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
            containerBuilder.RegisterType<ContainerProvider>().SingleInstance();
        }

        public static void SetConfigRoot()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            SetConfigRoot();

            // Add logging
            serviceCollection.AddLogging();

            //Build DB Connection
            serviceCollection.AddDbContextFactory<DatabaseContext>(BuildDbOptions);

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(ConfigurationRoot);
        }

    }
}
