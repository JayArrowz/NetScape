using Autofac;
using NetScape.Abstractions.Interfaces.World;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Modules.World
{
    public class WorldModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<World>().As<IWorld>().As<IStartable>().SingleInstance();
            builder.RegisterType<PlayerEntityList>().As<IEntityList<Player>>().SingleInstance();
            base.Load(builder);
        }
    }
}
