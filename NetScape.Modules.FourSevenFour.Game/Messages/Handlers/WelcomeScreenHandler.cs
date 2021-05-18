using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages;
using System;
using System.Threading.Tasks;
using static NetScape.Modules.Messages.Models.FourSevenFourDecoderMessages.Types;
using static NetScape.Modules.Messages.Models.FourSevenFourEncoderMessages.Types;

namespace NetScape.Modules.FourSevenFour.Game.Messages.Handlers
{
    [MessageHandler]
    public class WelcomeScreenHandler
    {
        private readonly IProtoMessageSender _messageSender;

        public WelcomeScreenHandler(IProtoMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [Message(typeof(ClickButtonMessage), nameof(Filter))]
        public async Task OnWelcomeScreenClick(DecoderMessage<ClickButtonMessage> decoderMessage)
        {
            var player = decoderMessage.Player;
            var buttonPackedId = decoderMessage.Message.ButtonId;
            switch(buttonPackedId)
            {
                case 6:
                    await _messageSender.SendAsync(player, new SendInterfaceMessage { InterfaceId = 548 });
                    player.UpdateAppearance();
                    return;
            }
        }

        public Predicate<DecoderMessage<ClickButtonMessage>> Filter { get; } = e => e.Message.InterfaceId == 378;
    }
}
