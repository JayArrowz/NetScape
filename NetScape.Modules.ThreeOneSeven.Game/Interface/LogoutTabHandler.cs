using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Models;
using System.Threading.Tasks;
using static NetScape.Modules.Messages.Models.ThreeOneSevenDecoderMessages.Types;
using static NetScape.Modules.Messages.Models.ThreeOneSevenEncoderMessages.Types;

namespace NetScape.Modules.ThreeOneSeven.Game.Interface
{
    [MessageHandler]
    public class LogoutTabHandler
    {
        private readonly IProtoMessageSender _protoMessageSender;
        private readonly IPlayerSerializer _playerSerializer;
        public LogoutTabHandler(IProtoMessageSender protoMessageSender, IPlayerSerializer playerSerializer)
        {
            _protoMessageSender = protoMessageSender;
            _playerSerializer = playerSerializer;
        }

        [Message(typeof(ButtonMessage))]
        public async Task OnLogoutClick(DecoderMessage<ButtonMessage> buttonMessage)
        {
            if (buttonMessage.Message.InterfaceId == 2458)
            {
                await _playerSerializer.AddOrUpdateAsync(buttonMessage.Player);
                await _protoMessageSender.SendAsync(buttonMessage.Player, new LogoutMessage());
            }
        }
    }
}
