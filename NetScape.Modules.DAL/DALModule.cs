using Autofac;
using NetScape.Abstractions.FileSystem;

namespace NetScape.Modules.DAL
{
    public class DALModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EntityFrameworkPlayerRepository>().As<IPlayerRepository>();
        }
    }
}
