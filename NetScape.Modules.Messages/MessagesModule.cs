using Autofac;
using Google.Protobuf;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Decoders;
using NetScape.Modules.Messages.Decoders.Handlers;
using NetScape.Modules.Messages.Models;
using System;
using System.Linq;

namespace NetScape.Modules.Messages
{
    public class MessagesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProvider>().As<IMessageProvider>().SingleInstance();
            builder.RegisterType<ProtoMessageSender>().As<IProtoMessageSender>().SingleInstance();
            builder.RegisterType<MessageAttributeHandler>().SingleInstance();
            builder.RegisterBuildCallback(t => t.Resolve<MessageAttributeHandler>().Start());

            builder.RegisterType<ProtoEncoder>();

            builder.RegisterType<MessageFrameEncoder>();
            builder.RegisterType<MessageHeaderDecoder>();
            builder.RegisterType<ProtoMessageCodecHandler>().As<IStartable>().AsSelf().SingleInstance();
            builder.RegisterType<MessageChannelHandler>();

            #region Decoders
            builder.RegisterType<MessageChannelHandler>();
            typeof(MessageCodec).Assembly.ExportedTypes.Where(t => t.IsAssignableTo<IMessage>())
                .ToList()
                .ForEach(type =>
                {
                    Type messageHandlerType = typeof(MessageDecoderBase<>).MakeGenericType(type);
                    builder.RegisterType(messageHandlerType)
                    .AsImplementedInterfaces()
                    .SingleInstance();
                });

            builder.RegisterAssemblyTypes(typeof(MessagesModule).Assembly)
                .AsClosedTypesOf(typeof(IMessageDecoder<>))
                .As<IMessageDecoder>()
                .SingleInstance();
            base.Load(builder);
            #endregion

            #region Handlers
            builder.RegisterType<WalkingQueueMessageHandler>().SingleInstance();
            #endregion
        }
    }
}
