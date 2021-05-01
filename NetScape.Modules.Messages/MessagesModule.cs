using Autofac;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Models;
using System;
using System.Linq;
using System.Reflection;

namespace NetScape.Modules.Messages
{
    public class MessagesModule : Autofac.Module
    {
        private readonly Type[] _messageCodecTypes;

        public MessagesModule(params Type[] messageCodecTypes)
        {
            _messageCodecTypes = messageCodecTypes;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProvider>().As<IMessageProvider>().SingleInstance();
            builder.RegisterType<ProtoMessageSender>().As<IProtoMessageSender>().SingleInstance();
            builder.RegisterType<MessageAttributeHandler>().SingleInstance();
            builder.RegisterBuildCallback(t => t.Resolve<MessageAttributeHandler>().Start());

            builder.RegisterType<ProtoEncoder>();

            builder.RegisterType<MessageFrameEncoder>();
            builder.RegisterType<MessageHeaderDecoder>();
            builder.RegisterInstance(new ProtoMessageCodecHandler(_messageCodecTypes))
                .As<IStartable>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<MessageChannelHandler>();

            #region Decoders
            typeof(MessageCodec).Assembly.ExportedTypes.Where(t => t.IsAssignableTo<IMessage>())
                .ToList()
                .ForEach(type =>
                {
                    var descriptor = (MessageDescriptor)type.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static).GetValue(null, null); // get the static property Descriptor
                    var codecExists = descriptor.CustomOptions.TryGetMessage<MessageCodec>(2001, out var messageCodec);
                    if (codecExists && !messageCodec.Custom)
                    {
                        Type messageHandlerType = typeof(MessageDecoderBase<>).MakeGenericType(type);
                        builder.RegisterType(messageHandlerType)
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    }
                });
            base.Load(builder);
            #endregion
        }
    }
}
