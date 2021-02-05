using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.Game.Interface
{
    public interface ITabManager
    {
        Task SetTabAsync(Model.Game.Player player, int tabId, int interfaceId);

        int[] Default { get; }
    }
}
