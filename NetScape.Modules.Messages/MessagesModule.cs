using Autofac;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Decoders;

namespace NetScape.Modules.Messages
{
    public class MessagesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProvider>().As<IMessageProvider>().SingleInstance();
            builder.RegisterType<MessageFrameEncoder>();
            builder.RegisterType<MessageHeaderDecoder>();
            builder.RegisterType<WalkingQueueMessage>().As<IMessageDecoder>().SingleInstance();
            builder.RegisterType<MessageChannelHandler>();
            base.Load(builder);
        }
    }
}
