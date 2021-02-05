using Autofac;
using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Modules.Game.Interface;
using NetScape.Modules.Game.Player;

namespace NetScape.Modules.Game
{
    public class GameModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TabManager>().As<ITabManager>();
            builder.RegisterType<PlayerInitializer>().As<IPlayerInitializer>();
            base.Load(builder);
        }
    }
}
