using NetScape.Abstractions.Model.Messages;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.ThreeOneSeven.Game.Messages.Handlers
{
    [MessageHandler]
    public class EmoteTabButtonsHandler
    {
        private Dictionary<int, int> ButtonAnimationMap { get; } = new Dictionary<int, int>
        {
            { 168, 855 },
            { 169, 856 },
            { 162, 857 },
            { 164, 858 },
        };

        [Message(typeof(ThreeOneSevenDecoderMessages.Types.ButtonMessage), nameof(CanExecute))]
        public void OnButtonClick(DecoderMessage<ThreeOneSevenDecoderMessages.Types.ButtonMessage> message)
        {
            var buttonId = message.Message.InterfaceId;
            int animation = ButtonAnimationMap[buttonId];
            message.Player.SendAnimation(new Animation(animation));
        }

        public bool CanExecute(DecoderMessage<ThreeOneSevenDecoderMessages.Types.ButtonMessage> message)
        {
            return ButtonAnimationMap.ContainsKey(message.Message.InterfaceId);
        }
    }
}
