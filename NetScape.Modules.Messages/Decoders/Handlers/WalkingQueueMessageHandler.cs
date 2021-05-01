using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game.Walking;
using NetScape.Modules.Messages.Models;

namespace NetScape.Modules.Messages.Decoders.Handlers
{
    [MessageHandler]
    public class WalkingQueueMessageHandler
    {
        private readonly WalkingQueueHandler _walkingQueueHandler;
        public WalkingQueueMessageHandler(WalkingQueueHandler walkingQueueHandler)
        {
            _walkingQueueHandler = walkingQueueHandler;
        }

        [Message(typeof(ThreeOneSevenDecoderMessages.Types.WalkingQueueMessage))]
        public void OnWalkQueueMessage(ThreeOneSevenDecoderMessages.Types.WalkingQueueMessage message)
        {
           /* var player = message.Player;
            var positions = message.Positions;

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
            player.WalkingQueue.Running = message.Run || player.WalkingQueue.Running;*/
        }
    }
}
