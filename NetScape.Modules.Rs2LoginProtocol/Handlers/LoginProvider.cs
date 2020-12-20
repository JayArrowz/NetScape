using System;
using NetScape.Abstractions.Interfaces.Login;
using DotNetty.Transport.Channels;
using Serilog;
using Autofac;
using NetScape.Abstractions.Interfaces;

namespace NetScape.Modules.LoginProtocol.Handlers
{
    public class LoginProvider : ILoginProvider
    {
        private readonly IContainer _container;

        public LoginProvider(ContainerProvider containerProvider)
        {
            _container = containerProvider.Container;
        }

        public Func<IChannelHandler[]> Handlers => () => new IChannelHandler[] {
            _container.Resolve<HandshakeDecoder>() 
        };
    }
}
