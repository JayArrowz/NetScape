using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Messages;
using System.Threading.Tasks;
using static NetScape.Modules.Messages.Models.FourSevenFourEncoderMessages.Types;

namespace NetScape.Modules.FourSevenFour.Game.Interface
{
    public class TabManager : ITabManager
    {
        private readonly IProtoMessageSender _protoMessageSender;

        public TabManager(IProtoMessageSender protoMessageSender)
        {
            _protoMessageSender = protoMessageSender;
        }

        public int[] TabIds { get; } = new int[] { 90, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112 };
        public int[] Default { get; } = new int[] { 137, 92, 320, 274, 149, 387, 271, 192, 589, 550, 551, 182, 261, 464, 239 };

        public Task SetTabAsync(Abstractions.Model.Game.Player player, int tabId, int interfaceId)
        {
            var switchTabMessage = new OpenInterfaceMessage
            {
                InterfaceId = interfaceId,
                Window = 548, //Main window
                Position = TabIds[tabId],
                Walkable = true
            };
            return _protoMessageSender.SendAsync(player, switchTabMessage);
        }
    }
}
