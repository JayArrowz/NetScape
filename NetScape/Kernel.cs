using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Modules.Logging.SeriLog;
using NetScape.Modules.LoginProtocol;
using NetScape.Modules.Server;
using NetScape.Modules.Cache;
using System;
using System.IO;
using NetScape.Modules.DAL;

namespace NetScape
{
    public class Kernel
    {
        public static IConfigurationRoot ConfigurationRoot { get; set; }
        public static void Main()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            ConfigureAutofac(containerBuilder);

            var container = containerBuilder.Build();
            container.Resolve<ContainerProvider>().Container = container;
            var serviceProvider = new AutofacServiceProvider(container);

            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                var gameServer = serviceProvider.GetRequiredService<IGameServer>();
                _ = gameServer.BindAsync();
                Console.ReadLine();
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
            containerBuilder.RegisterModule(new SeriLogModule(ConfigurationRoot));
            containerBuilder.RegisterModule(new LoginModule());
            containerBuilder.RegisterModule(new CacheModule());
            containerBuilder.RegisterModule(new DALModule());
            containerBuilder.RegisterModule(new GameServerModule(ConfigurationRoot["BindAddr"], ushort.Parse(ConfigurationRoot["BindPort"])));
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
