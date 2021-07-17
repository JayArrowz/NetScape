using Autofac;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Modules.DAL
{
    public class DALModule<TPlayer> : Module where TPlayer : Player, new()
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EntityFrameworkPlayerRepository<TPlayer>>().As<IPlayerRepository>();
        }
    }
}
