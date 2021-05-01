using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages;
using System;
using System.Linq;
using static NetScape.Modules.Messages.Models.ThreeOneSevenDecoderMessages.Types;

namespace NetScape.Modules.ThreeOneSeven.Game.Interface
{

    [MessageHandler]
    public class RunButtonsHandler
    {
        private static readonly int[] _runButtons = new int[] { 152, 153 };

        [Message(typeof(ButtonMessage), nameof(Filter))]
        public void OnButtonClick(DecoderMessage<ButtonMessage> decoderMessage)
        {
            decoderMessage.Player.WalkingQueue.Running = decoderMessage.Message.InterfaceId == 153;
        }

        public Predicate<DecoderMessage<ButtonMessage>> Filter { get; } = e => _runButtons.Contains(e.Message.InterfaceId);
    }
}
