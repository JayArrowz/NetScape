using Autofac;
using NetScape.Abstractions.Interfaces.Region.Collision;

namespace NetScape.Modules.Region.Collision
{
    public class CollisionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CollisionManager>().As<ICollisionManager>();
            base.Load(builder);
        }
    }
}
