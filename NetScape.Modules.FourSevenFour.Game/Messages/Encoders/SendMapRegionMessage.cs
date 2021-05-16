using DotNetty.Buffers;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetScape.Modules.FourSevenFour.Game.Messages.Encoders
{
    public class SendMapRegionMessage : IEncoderMessage<MessageFrame>
    {
        public Player Player { get; }

        public SendMapRegionMessage(Player player)
        {
            Player = player;
        }

        public MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            var messageFrameBuilder = new MessageFrameBuilder(alloc, 61, FrameType.VariableShort);
            var playerPos = Player.Position;
            var clearRegion = false;
            
            var regionX = playerPos.RegionX;
            var regionY = playerPos.RegionY;
            var absoluteRegionX = (regionX + 6) / 8;
            var absoluteRegionY = (regionY + 6) / 8;

            if (
                ((absoluteRegionX == 48 || absoluteRegionX == 49) && absoluteRegionY == 48) ||
                (absoluteRegionX == 48 && absoluteRegionY == 148)
               )
            {
                clearRegion = true;
            }
            messageFrameBuilder.Put(MessageType.Short, DataOrder.Little, DataTransformation.Add, playerPos.LocalY);
            messageFrameBuilder.Put(MessageType.Byte, playerPos.Height);
            messageFrameBuilder.Put(MessageType.Short, regionX + 6);
            for (int xCalc = regionX / 8; xCalc <= (regionX + 12) / 8; xCalc++)
            {
                for (int yCalc = regionY / 8; yCalc <= (regionY + 12) / 8; yCalc++)
                {
                    int regionId = yCalc + (xCalc << 8); // 1786653352
                    if (clearRegion || yCalc != 49 && yCalc != 149 && yCalc != 147
                            && xCalc != 50 && (xCalc != 49 || yCalc != 47))
                    {
                        messageFrameBuilder.Put(MessageType.Int, DataOrder.Little, 0);
                        messageFrameBuilder.Put(MessageType.Int, DataOrder.Little, 0);
                        messageFrameBuilder.Put(MessageType.Int, DataOrder.Little, 0);
                        messageFrameBuilder.Put(MessageType.Int, DataOrder.Little, 0);
                    }
                }
            }

            messageFrameBuilder.Put(MessageType.Short, DataOrder.Big, DataTransformation.Add, playerPos.LocalX);
            messageFrameBuilder.Put(MessageType.Short, regionY + 6);
            return messageFrameBuilder.ToMessageFrame();
        }
    }
}
