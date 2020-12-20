using System;
using DotNetty.Transport.Channels;

namespace NetScape.Abstractions.Interfaces.Login
{
    public interface ILoginProvider
    {
        Func<IChannelHandler[]> Handlers { get; }
    }
}
