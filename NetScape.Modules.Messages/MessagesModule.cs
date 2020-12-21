using Autofac;
using NetScape.Abstractions.Interfaces.Messages;

namespace NetScape.Modules.Messages
{
    public class MessagesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProvider>().As<IMessageProvider>().SingleInstance();
            builder.RegisterType<MessageFrameEncoder>();
            builder.RegisterType<MessageHeaderDecoder>();
            base.Load(builder);
        }
    }
}
