using System;
using NetScape.Abstractions.Interfaces.IO.Login;
using DotNetty.Transport.Channels;
using Serilog;

namespace NetScape.Modules.LoginProtocol.IO.Login
{
    public class LoginProvider : ILoginProvider
    {
        private readonly ILogger _logger;

        public LoginProvider(ILogger logger)
        {
            _logger = logger;
        }

        public Func<IChannelHandler[]> Handlers => () => new IChannelHandler[] { 
            new HandshakeDecoder(_logger) 
        };
    }
}
