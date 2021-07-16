using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using NetScape.Modules.Messages.Models;
using System.Linq;

namespace NetScape.Modules.FourSevenFour.Game.Messages.Decoders
{
    public class WalkingQueueMessageDecoder : MessageDecoderBase<FourSevenFourDecoderMessages.Types.WalkingQueueMessage>
    {
        public override int[] Ids { get; } = new int[] { 11, 46, 59 };
        public override FrameType FrameType { get; } = FrameType.VariableByte;

        protected override FourSevenFourDecoderMessages.Types.WalkingQueueMessage Decode(Abstractions.Model.Game.Player player, MessageFrame frame)
        {
            var reader = new MessageFrameReader(frame);
            var length = frame.Payload.ReadableBytes;

            if (frame.Id == 11)
            {
                length -= 14; // strip off anti-cheat data
            }

            int steps = (length - 5) / 2;
            int[,] path = new int[steps, 2];
            for (int i = 0; i < steps; i++)
            {
                path[i, 0] = (int)reader.GetSigned(MessageType.Byte);
                path[i, 1] = (int)reader.GetSigned(MessageType.Byte, DataTransformation.Subtract);
            }
            int x = (int)reader.GetUnsigned(MessageType.Short, DataTransformation.Add);
            int y = (int)reader.GetUnsigned(MessageType.Short, DataOrder.Little);
            var run = reader.GetUnsigned(MessageType.Byte, DataTransformation.Negate) == 1;

            var positions = new Position[steps + 1];
            positions[0] = new Position(x, y);
            for (int i = 0; i < steps; i++)
            {
                positions[i + 1] = new Position(path[i, 0] + x, path[i, 1] + y);
            }
            FourSevenFourDecoderMessages.Types.WalkingQueueMessage walkingQueueMessage = new() { Run = run, };
            walkingQueueMessage.X.Add(positions.Select(t => t.X));
            walkingQueueMessage.Y.Add(positions.Select(t => t.Y));
            return walkingQueueMessage;
        }
    }
}
