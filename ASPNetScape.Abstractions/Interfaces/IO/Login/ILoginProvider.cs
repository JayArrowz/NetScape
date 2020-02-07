using System;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Abstractions.Interfaces.IO.Login
{
    public interface ILoginProvider
    {
        Func<IChannelHandler[]> Handlers { get; }
    }
}
