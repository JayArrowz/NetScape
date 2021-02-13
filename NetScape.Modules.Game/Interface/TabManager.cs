using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Modules.Messages.Encoders;
using System.Threading.Tasks;

namespace NetScape.Modules.Game.Interface
{
    public class TabManager : ITabManager
    {
        public int[] Default { get; } = new int[] { 2423, 3917, 638, 3213, 1644, 5608, 1151, -1, 5065, 5715, 2449, 904, 147, 962, };

        public Task SetTabAsync(Abstractions.Model.Game.Player player, int tabId, int interfaceId)
        {
            var switchTabMessage = new SwitchTabInterfaceMessage
            {
                InterfaceId = (ushort)interfaceId,
                TabId = (byte)tabId
            };
            return player.SendAsync(switchTabMessage);
        }
    }
}
