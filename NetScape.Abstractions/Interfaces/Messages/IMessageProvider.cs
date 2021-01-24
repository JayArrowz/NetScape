using NetScape.Abstractions.Interfaces.IO;

namespace NetScape.Abstractions.Interfaces.Messages
{
    /// <summary>
    /// Provides channel handlers for after the user has been authenticated
    /// </summary>
    /// <seealso cref="NetScape.Abstractions.Interfaces.IO.IChannelHandlerProvider" />
    public interface IMessageProvider : IChannelHandlerProvider
    {
    }
}
