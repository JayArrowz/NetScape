using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Interfaces.Messages
{
    /// <summary>
    /// A handler which is aware of the player
    /// </summary>
    public interface IPlayerAwareHandler
    {
        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public Player Player { get; set; }
    }
}
