using System;
using ASPNetScape.Abstractions.Interfaces.IO.Login;
using DotNetty.Transport.Channels;
using Serilog;

namespace ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Login
{
    public class LoginProvider : ILoginProvider
    {
        private readonly ILogger _logger;

        public LoginProvider(ILogger logger)
        {
            _logger = logger;
        }

        public Func<IChannelHandler[]> Handlers => () => new IChannelHandler[] { 
            new LoginHandshakeDecoder(_logger) 
        };
    }
}
