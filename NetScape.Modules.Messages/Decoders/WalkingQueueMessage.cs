using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Game.Walking;
using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Modules.Messages.Decoders
{
    public class WalkingQueueMessage : IMessageDecoder
    {
        private readonly WalkingQueueHandler _walkingQueueHandler;

        public WalkingQueueMessage(WalkingQueueHandler walkingQueueHandler)
        {
            _walkingQueueHandler = walkingQueueHandler;
        }

        public int[] Ids { get; } = new int[] { 248, 164, 98 };
        public FrameType FrameType => FrameType.VariableByte;
        public bool Run { get; private set; }
        public Position[] Positions { get; private set; }

        public void Decode(Player player, MessageFrame frame)
        {
            var reader = new MessageFrameReader(frame);
            var length = frame.Payload.ReadableBytes;

            if (frame.Id == 248)
            {
                length -= 14; // strip off anti-cheat data
            }

            int steps = (length - 5) / 2;
            int[,] path = new int[steps, 2];
            int x = (int)reader.GetUnsigned(MessageType.Short, DataOrder.Little, DataTransformation.Add);
            for (int i = 0; i < steps; i++)
            {
                path[i, 0] = (int)reader.GetSigned(MessageType.Byte);
                path[i, 1] = (int)reader.GetSigned(MessageType.Byte);
            }
            int y = (int)reader.GetUnsigned(MessageType.Short, DataOrder.Little);
            Run = reader.GetUnsigned(MessageType.Byte, DataTransformation.Negate) == 1;

            Positions = new Position[steps + 1];
            Positions[0] = new Position(x, y);
            for (int i = 0; i < steps; i++)
            {
                Positions[i + 1] = new Position(path[i, 0] + x, path[i, 1] + y);
            }

            for (int index = 0; index < Positions.Length; index++)
            {
                Position step = Positions[index];
                if (index == 0)
                {
                    _walkingQueueHandler.AddFirstStep(player, step);
                }
                else
                {
                    _walkingQueueHandler.AddStep(player, step);
                }
            }

            player.WalkingQueue.Running = Run || player.WalkingQueue.Running;
        }
    }
}
