using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Modules.Messages.Encoders;
using System.Threading.Tasks;

namespace NetScape.Modules.Game.Player
{
    public class PlayerInitializer : IPlayerInitializer
    {
        private readonly ITabManager _tabManager;

        public PlayerInitializer(ITabManager tabManager)
        {
            _tabManager = tabManager;
        }

        public async Task InitializeAsync(Abstractions.Model.Game.Player player)
        {
            var initMessage = new IdAssignmentMessage { IsMembers = 1, NewId = 1 };
            await player.SendAsync(initMessage);
            player.UpdateAppearance();

            var defaultTabs = _tabManager.Default;
            for (int tab = 0; tab < defaultTabs.Length; tab++)
            {
                var interfaceId = defaultTabs[tab];
                _ = _tabManager.SetTabAsync(player, tab, interfaceId);
            }
        }
    }
}
