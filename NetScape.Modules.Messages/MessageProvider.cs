using Autofac;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.Messages;
using System;
using NetScape.Abstractions.Server;

namespace NetScape.Modules.Messages
{
    public class MessageProvider : IMessageProvider
    {
        private readonly IContainer _container;

        public MessageProvider(ContainerProvider containerProvider)
        {
            _container = containerProvider.Container;
        }

        public Func<IChannelHandler[]> Provide => () => new IChannelHandler[] {
            _container.Resolve<MessageChannelHandler>(),
            _container.Resolve<MessageHeaderDecoder>(),
            _container.Resolve<MessageFrameEncoder>(),
            _container.Resolve<ProtoEncoder>(),
        };
    }
}
