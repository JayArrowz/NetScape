using Autofac;
using NetScape.Abstractions.Interfaces.Region;

namespace NetScape.Modules.Region
{
    public class RegionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegionRepository>().As<IRegionRepository>().SingleInstance();
            base.Load(builder);
        }
    }
}
