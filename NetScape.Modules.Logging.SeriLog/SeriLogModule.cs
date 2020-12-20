using System;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Events;
using Module = Autofac.Module;

namespace NetScape.Modules.Logging.SeriLog
{
    public sealed class SeriLogModule : Module
    {
        private readonly IConfigurationRoot _configurationRoot;

        public SeriLogModule(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(_configurationRoot);

            var logger = loggerConfig.CreateLogger()
                .ForContext(MethodBase.GetCurrentMethod().DeclaringType);
            Log.Logger = logger;
            builder.RegisterInstance(logger).As<ILogger>();
        }
    }
}
