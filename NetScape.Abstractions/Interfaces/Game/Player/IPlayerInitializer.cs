using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.Game.Player
{
    /// <summary>
    /// Initializes a player after login is complete.
    /// </summary>
    public interface IPlayerInitializer
    {
        /// <summary>
        /// Initializes a <see cref="Model.Game.Player"/> after <see cref="Login.ILoginProvider"/>
        /// authenticates the player successfully.
        /// </summary>
        /// <param name="player">The player.</param>
        Task InitializeAsync(Model.Game.Player player);
    }
}
