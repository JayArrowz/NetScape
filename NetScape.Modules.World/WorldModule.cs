using Autofac;
using NetScape.Abstractions.Interfaces.World;

namespace NetScape.Modules.World
{
    public class WorldModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<World>().As<IWorld>().As<IStartable>().SingleInstance();
            base.Load(builder);
        }
    }
}
