using Autofac;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game.Walking;
using NetScape.Modules.Messages.Decoders.Messages;
using System;

namespace NetScape.Modules.Messages.Decoders.Handlers
{
    public class WalkingQueueMessageHandler : IStartable, IDisposable
    {
        private readonly IMessageDecoder<WalkingQueueMessage> _walkingQueueDecoder;
        private readonly WalkingQueueHandler _walkingQueueHandler;
        public WalkingQueueMessageHandler(IMessageDecoder<WalkingQueueMessage> walkingQueueDecoder, WalkingQueueHandler walkingQueueHandler)
        {
            _walkingQueueDecoder = walkingQueueDecoder;
            _walkingQueueHandler = walkingQueueHandler;
        }

        private IDisposable _subscription;

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public void Start()
        {
            _subscription = _walkingQueueDecoder.Subscribe(OnWalkQueueMessage);
        }

        private void OnWalkQueueMessage(WalkingQueueMessage message)
        {
            var player = message.Player;
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
            player.WalkingQueue.Running = message.Run || player.WalkingQueue.Running;
        }
    }
}
