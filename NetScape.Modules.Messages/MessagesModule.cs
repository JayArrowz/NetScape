using Autofac;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Region;

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
            base.Load(builder);
        }
    }
}
