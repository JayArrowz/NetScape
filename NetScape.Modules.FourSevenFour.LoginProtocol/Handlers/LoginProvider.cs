using Autofac;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.Login;
using System;
using NetScape.Abstractions.Server;

namespace NetScape.Modules.FourSevenFour.LoginProtocol.Handlers
{
    public class LoginProvider : ILoginProvider
    {
        private readonly IContainer _container;

        public LoginProvider(ContainerProvider containerProvider)
        {
            _container = containerProvider.Container;
        }

        public Func<IChannelHandler[]> Provide => () => new IChannelHandler[] {
            _container.Resolve<HandshakeDecoder>()
        };
    }
}
