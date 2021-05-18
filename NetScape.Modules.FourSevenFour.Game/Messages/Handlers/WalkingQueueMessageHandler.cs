using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Models;
using System.Linq;

namespace NetScape.Modules.FourSevenFour.Game.Messages.Handlers { 

    [MessageHandler]
    public class WalkingQueueMessageHandler
    {
        private readonly WalkingQueueHandler _walkingQueueHandler;
        public WalkingQueueMessageHandler(WalkingQueueHandler walkingQueueHandler)
        {
            _walkingQueueHandler = walkingQueueHandler;
        }

        [Message(typeof(FourSevenFourDecoderMessages.Types.WalkingQueueMessage))]
        public void OnWalkQueueMessage(DecoderMessage<FourSevenFourDecoderMessages.Types.WalkingQueueMessage> decoderMessage)
        {
            var player = decoderMessage.Player;
            var message = decoderMessage.Message;
            var positions =  Enumerable.Range(0, message.X.Count)
                .Select(t => new Position
                {
                    X = message.X[t],
                    Y = message.Y[t]
                }).ToArray();

            for (int index = 0; index < positions.Length; index++)
            {
                Position step = positions[index];
                if (index == 0)
                {
                    _walkingQueueHandler.AddFirstStep(player, step);
                }
                else
                {
                    _walkingQueueHandler.AddStep(player, step);
                }
            }
            player.WalkingQueue.Running = message.Run || player.WalkingQueue.Running;
        }
    }
}
