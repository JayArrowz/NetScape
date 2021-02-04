using Autofac;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Decoders;
using NetScape.Modules.Messages.Decoders.Handlers;
using NetScape.Modules.Messages.Decoders.Messages;

namespace NetScape.Modules.Messages
{
    public class MessagesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProvider>().As<IMessageProvider>().SingleInstance();
            builder.RegisterType<MessageFrameEncoder>();
            builder.RegisterType<MessageHeaderDecoder>();
            builder.RegisterType<MessageChannelHandler>();

            #region Decoders
            builder.RegisterType<WalkingQueueMessageDecoder>()
                .AsImplementedInterfaces().SingleInstance();
            base.Load(builder);
            #endregion

            #region Handlers
            builder.RegisterType<WalkingQueueMessageHandler>().AsImplementedInterfaces().SingleInstance();
            #endregion
        }
    }
}
