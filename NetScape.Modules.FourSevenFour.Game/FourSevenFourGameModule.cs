using Autofac;
using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.FourSevenFour.Game.Interface;
using NetScape.Modules.FourSevenFour.Game.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.FourSevenFour.Game
{
    public class FourSevenFourGameModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TabManager>().As<ITabManager>();
            builder.RegisterType<PlayerInitializer>().As<IPlayerInitializer>();

            #region Handlers
            builder.RegisterAssemblyTypes(typeof(FourSevenFourGameModule).Assembly)
                .AsClosedTypesOf(typeof(IMessageDecoder<>))
                .As<IMessageDecoder>()
                .SingleInstance();
            #endregion
            base.Load(builder);
        }
    }
}
