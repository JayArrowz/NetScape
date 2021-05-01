using Autofac;
using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.ThreeOneSeven.Game.Interface;
using NetScape.Modules.ThreeOneSeven.Game.Messages.Handlers;
using NetScape.Modules.ThreeOneSeven.Game.Players;

namespace NetScape.Modules.Game
{
    public class ThreeOneSevenGameModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TabManager>().As<ITabManager>();
            builder.RegisterType<PlayerInitializer>().As<IPlayerInitializer>();

            #region Handlers
            builder.RegisterType<WalkingQueueMessageHandler>().SingleInstance();


            builder.RegisterAssemblyTypes(typeof(ThreeOneSevenGameModule).Assembly)
                .AsClosedTypesOf(typeof(IMessageDecoder<>))
                .As<IMessageDecoder>()
                .SingleInstance();
            #endregion
            base.Load(builder);
        }
    }
}
