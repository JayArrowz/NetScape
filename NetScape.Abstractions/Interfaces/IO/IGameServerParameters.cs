namespace NetScape.Abstractions.Interfaces.IO
{
    /// <summary>
    /// Parameters of the game server
    /// </summary>
    public interface IGameServerParameters
    {
        /// <summary>
        /// Gets or sets the bind address.
        /// </summary>
        /// <value>
        /// The bind address.
        /// </value>
        string BindAddress { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        ushort Port { get; set; }
    }
}
