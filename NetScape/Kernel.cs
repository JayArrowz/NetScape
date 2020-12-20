using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.IO;
using NetScape.Modules.Logging.SeriLog;
using NetScape.Modules.LoginProtocol;
using NetScape.Modules.Server;
using NetScape.Modules.SevenOneEight.Cache;
using System;
using System.IO;

namespace NetScape
{
    public class Kernel
    {
        private static IConfigurationRoot _configurationRoot;
        public static void Main(string[] args)
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

        private static void ConfigureAutofac(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new SeriLogModule(_configurationRoot));
            containerBuilder.RegisterModule(new LoginModule());
            containerBuilder.RegisterModule(new CacheModule());
            containerBuilder.RegisterModule(new GameServerModule("127.0.0.1", 43594));
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
            containerBuilder.RegisterType<ContainerProvider>().SingleInstance();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddLogging();

            // Build configuration
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(_configurationRoot);
        }

    }
}
