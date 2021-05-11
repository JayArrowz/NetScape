using Autofac;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.Login;
using System;

namespace NetScape.Modules.Osrs.LoginProtocol
{
    public class LoginProvider : ILoginProvider
    {
        private readonly ContainerProvider _containerProvider;

        public LoginProvider(ContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }
        public Func<IChannelHandler[]> Provide => () =>
                new IChannelHandler[] { 
                    _containerProvider.Container.Resolve<HandshakeDecoder>() 
                };
    }
}
