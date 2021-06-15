using NetScape.Abstractions.Interfaces.Cache;
using NetScape.Modules.Cache;
using NetScape.Modules.Cache.RuneTek5;
using Autofac;

namespace NetScape.Modules.Cache
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileStore>().As<IFileStore>().As<IStartable>().SingleInstance();
            builder.RegisterType<RuneTek5Cache>().As<IReferenceTableCache>().As<IStartable>().SingleInstance();
        }
    }
}
