using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.IO
{
    public interface IGameServer : IAsyncDisposable
    {
        /// <summary>
        /// The Server Channel
        /// </summary>
        IChannel Channel { get; set; }

        /// <summary>
        /// Binds the game server
        /// </summary>
        Task BindAsync();
    }
}
