using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.IO
{
    public interface IChannelHandlerProvider
    {
        Func<IChannelHandler[]> Provide { get; }
    }
}
