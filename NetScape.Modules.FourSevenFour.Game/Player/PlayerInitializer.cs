using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Game.Player;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.FourSevenFour.Game.Messages.Encoders;
using System.Threading.Tasks;
using static NetScape.Modules.Messages.Models.FourSevenFourEncoderMessages.Types;

namespace NetScape.Modules.FourSevenFour.Game.Players
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
            //player.UpdateAppearance();
            _ = player.SendAsync(new SendMapRegionMessage(player)).ContinueWith(_ =>
            {

                var defaultTabs = _tabManager.Default;
                for (int tab = 0; tab < defaultTabs.Length; tab++)
                {
                    var interfaceId = defaultTabs[tab];
                    _ = _tabManager.SetTabAsync(player, tab, interfaceId);
                }
            });

            var openLoginScreenMessage = new SendInterfaceMessage
            {
                 InterfaceId = 549
            };

            _ = _protoMessageSender.SendAsync(player, openLoginScreenMessage);
            _ = _protoMessageSender.SendAsync(player, new OpenInterfaceMessage
            {
                 Window = 549,
                 Position = 2,
                 InterfaceId = 378,
                 Walkable = true
            });
            _ = _protoMessageSender.SendAsync(player, new OpenInterfaceMessage
            {
                Window = 549,
                Position = 3,
                InterfaceId = 17,
                Walkable = true
            });
            return Task.CompletedTask;
        }
    }
}
