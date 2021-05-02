using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Models;
using System.Threading.Tasks;

namespace NetScape.Modules.ThreeOneSeven.Game.Players
{
    public class PlayerInitializer : IPlayerInitializer
    {
        private readonly ITabManager _tabManager;
        private readonly IProtoMessageSender _protoMessageSender;

        public PlayerInitializer(ITabManager tabManager, IProtoMessageSender protoMessageSender)
        {
            _tabManager = tabManager;
            _protoMessageSender = protoMessageSender;
        }

        public Task InitializeAsync(Abstractions.Model.Game.Player player)
        {
            var initMessage = new ThreeOneSevenEncoderMessages.Types.
                IdAssignmentMessage
            { IsMembers = true, NewId = 1 };
            _ = _protoMessageSender.SendAsync(player, initMessage);
            player.UpdateAppearance();

            var defaultTabs = _tabManager.Default;
            for (int tab = 0; tab < defaultTabs.Length; tab++)
            {
                var interfaceId = defaultTabs[tab];
                _ = _tabManager.SetTabAsync(player, tab, interfaceId);
            }
            return Task.CompletedTask;
        }
    }
}
