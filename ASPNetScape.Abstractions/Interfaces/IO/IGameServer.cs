using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;

namespace ASPNetScape.Abstractions.Interfaces.IO
{
    public interface IGameServer : IDisposable
    {
        /// <summary>
        /// Server Channel
        /// </summary>
        IChannel Channel { get; set; }

        /// <summary>
        /// Binds the game server
        /// </summary>
        Task BindAsync();
    }
}
