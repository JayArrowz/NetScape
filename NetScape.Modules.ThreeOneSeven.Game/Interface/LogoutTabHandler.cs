using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Models;
using System;
using System.Threading.Tasks;
using static NetScape.Modules.Messages.Models.ThreeOneSevenDecoderMessages.Types;
using static NetScape.Modules.Messages.Models.ThreeOneSevenEncoderMessages.Types;

namespace NetScape.Modules.ThreeOneSeven.Game.Interface
{
    [MessageHandler]
    public class LogoutTabHandler
    {
        private readonly IProtoMessageSender _protoMessageSender;
        private readonly IPlayerRepository _playerRepository;
        public LogoutTabHandler(IProtoMessageSender protoMessageSender, IPlayerRepository playerRepository)
        {
            _protoMessageSender = protoMessageSender;
            _playerRepository = playerRepository;
        }

        [Message(typeof(ButtonMessage), nameof(Filter))]
        public async Task OnLogoutClick(DecoderMessage<ButtonMessage> buttonMessage)
        {
            await _playerRepository.AddOrUpdateAsync(buttonMessage.Player);
            await _protoMessageSender.SendAsync(buttonMessage.Player, new LogoutMessage());
        }

        public Predicate<DecoderMessage<ButtonMessage>> Filter { get; } = (message) => message.Message.InterfaceId == 2458;
    }
}
