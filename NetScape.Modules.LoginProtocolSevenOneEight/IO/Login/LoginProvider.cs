using System;
using ASPNetScape.Abstractions.Interfaces.IO;
using ASPNetScape.Abstractions.Interfaces.IO.Login;
using DotNetty.Transport.Channels;
using Serilog;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Login
{
    public class LoginProvider : ILoginProvider
    {
        private readonly ILogger _logger;
        private readonly IJS5PacketSender _jS5PacketSender;

        public LoginProvider(ILogger logger, IJS5PacketSender jS5PacketSender)
        {
            _logger = logger;
            _jS5PacketSender = jS5PacketSender;
        }

        public Func<IChannelHandler[]> Handlers => () => new IChannelHandler[] { 
            new LoginHandshakeDecoder(_logger, _jS5PacketSender) 
        };
    }
}
