using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Login;
using System.Threading.Tasks;

namespace NetScape.Abstractions.FileSystem
{
    public interface IPlayerSerializer
    {
        /// <summary>
        /// Retrive player for name
        /// </summary>
        /// <param name="name">The player name.</param>
        /// <returns>Player</returns>
        Task<Player> GetAsync(string name);

        /// <summary>
        /// Retrive player for player credentials
        /// </summary>
        /// <param name="playerCredentials">The player credentials.</param>
        /// <returns>Player</returns>
        Task<Player> GetOrCreateAsync(PlayerCredentials playerCredentials);

        /// <summary>
        /// Adds player if the player does not exist, otherwise the player is updated
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Number of rows updated</returns>
        Task<int> AddOrUpdateAsync(Player player);
    }
}
