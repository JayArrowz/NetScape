using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages;
using Serilog;
using static NetScape.Modules.Messages.Models.ThreeOneSevenDecoderMessages.Types;

namespace NetScape.Modules.ThreeOneSeven.Game.Messages.Handlers
{
    [MessageHandler]
    public class CommandMessageHandler
    {

        [Message(typeof(CommandMessage))]
        public void OnCommand(DecoderMessage<CommandMessage> decoderMessage)
        {
            var commandMessage = decoderMessage.Message;
            Log.Logger.Information("Player {0} executed command {1}", decoderMessage.Player.Username, commandMessage.Command);
        }
    }
}
