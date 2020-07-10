using System;
using DotNetty.Transport.Channels;

namespace NetScape.Abstractions.Interfaces.IO.Login
{
    public interface ILoginProvider
    {
        Func<IChannelHandler[]> Handlers { get; }
    }
}
