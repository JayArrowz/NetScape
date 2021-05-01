using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using NetScape.Modules.Messages.Models;
using System.Linq;

namespace NetScape.Modules.Messages.Decoders
{
    public class WalkingQueueMessageDecoder : MessageDecoderBase<ThreeOneSevenDecoderMessages.Types.WalkingQueueMessage>
    {
        public override int[] Ids { get; } = new int[] { 248, 164, 98 };
        public override FrameType FrameType { get; } = FrameType.VariableByte;

        protected override ThreeOneSevenDecoderMessages.Types.WalkingQueueMessage Decode(Player player, MessageFrame frame)
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
            var run = reader.GetUnsigned(MessageType.Byte, DataTransformation.Negate) == 1;

            var positions = new Position[steps + 1];
            positions[0] = new Position(x, y);
            for (int i = 0; i < steps; i++)
            {
                positions[i + 1] = new Position(path[i, 0] + x, path[i, 1] + y);
            }
            ThreeOneSevenDecoderMessages.Types.WalkingQueueMessage walkingQueueMessage =  new() { Run = run, };
            walkingQueueMessage.Y.Add(positions.Select(t => t.X));
            walkingQueueMessage.Y.Add(positions.Select(t => t.Y));
            return walkingQueueMessage;
        }
    }
}
