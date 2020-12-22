using DotNetty.Transport.Channels;
using System;

namespace NetScape.Abstractions.Interfaces.IO
{
    /// <summary>
    ///  Provides channel handlers
    /// </summary>
    public interface IChannelHandlerProvider
    {
        Func<IChannelHandler[]> Provide { get; }
    }
}
