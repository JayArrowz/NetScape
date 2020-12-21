using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Login;
using System.Threading.Tasks;

namespace NetScape.Abstractions.FileSystem
{
    public interface IPlayerSerializer
    {
        Task<Player> GetAsync(string name);
        Task<Player> GetOrCreateAsync(PlayerCredentials playerCredentials);
        Task<int> AddOrUpdateAsync(Player player);
    }
}
