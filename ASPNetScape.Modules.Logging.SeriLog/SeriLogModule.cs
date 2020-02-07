using System;
using System.Reflection;
using Autofac;
using Serilog;
using Serilog.Events;
using Module = Autofac.Module;

namespace ASPNetScape.Modules.Logging.SeriLog
{
    public sealed class SeriLogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var loggingDir = AppDomain.CurrentDomain.GetData("DataDirectory") + "/Log-{Date}.txt";

            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(loggingDir,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] ({MachineName}/{ThreadId}) {Message} ({SourceContext}){NewLine}{Exception}");
            loggerConfig.MinimumLevel.Verbose();

            var logger = loggerConfig.CreateLogger()
                .ForContext(MethodBase.GetCurrentMethod().DeclaringType);

            builder.RegisterInstance(logger).As<ILogger>();
        }
    }
}
