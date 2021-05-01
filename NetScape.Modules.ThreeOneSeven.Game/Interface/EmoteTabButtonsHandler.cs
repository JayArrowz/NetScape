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
            { 168, 855 }
        };

        [Message(typeof(ThreeOneSevenDecoderMessages.Types.ButtonMessage))]
        public void OnButtonClick(DecoderMessage<ThreeOneSevenDecoderMessages.Types.ButtonMessage> message)
        {
            var buttonId = message.Message.InterfaceId;
            int? animation = ButtonAnimationMap.ContainsKey(buttonId) ? ButtonAnimationMap[buttonId] : null;
            if(animation.HasValue)
            {
                message.Player.SendAnimation(new Animation(animation.Value));
            }
        }
    }
}
