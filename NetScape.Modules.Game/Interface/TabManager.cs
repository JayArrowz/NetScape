using NetScape.Abstractions.Interfaces.Game.Interface;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Modules.Messages.Models;
using System.Threading.Tasks;

namespace NetScape.Modules.Game.Interface
{
    public class TabManager : ITabManager
    {
        private readonly IProtoMessageSender _protoMessageSender;

        public TabManager(IProtoMessageSender protoMessageSender)
        {
            _protoMessageSender = protoMessageSender;
        }

        public int[] Default { get; } = new int[] { 2423, 3917, 638, 3213, 1644, 5608, 1151, -1, 5065, 5715, 2449, 904, 147, 962, };

        public Task SetTabAsync(Abstractions.Model.Game.Player player, int tabId, int interfaceId)
        {
            var switchTabMessage = new ThreeOneSevenEncoderMessages.Types.SwitchTabInterfaceMessage
            {
                InterfaceId = interfaceId,
                TabId = tabId
            };
            return _protoMessageSender.SendAsync(player, switchTabMessage);
        }
    }
}
