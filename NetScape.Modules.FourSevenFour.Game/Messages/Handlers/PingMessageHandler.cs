using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.FourSevenFour.Game.Messages.Encoders;
using NetScape.Modules.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetScape.Modules.Messages.Models.FourSevenFourDecoderMessages.Types;

namespace NetScape.Modules.FourSevenFour.Game.Messages.Handlers
{
    [MessageHandler]
    public class PingMessageHandler
    {
        [Message(typeof(PingMessage))]
        public void OnPingMessage(DecoderMessage<PingMessage> message)
        {
            _ = message.Player.SendAsync(new SendPingMessage(message.Player));
        }
    }
}
