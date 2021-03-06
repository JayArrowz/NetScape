﻿using DotNetty.Buffers;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Modules.FourSevenFour.Game.Messages.Encoders
{
    public class SendPingMessage : IEncoderMessage<MessageFrame>
    {
        public Abstractions.Model.Game.Player Player { get; }

        public SendPingMessage(Abstractions.Model.Game.Player player)
        {
            Player = player;
        }

        public MessageFrame ToMessage(IByteBufferAllocator alloc)
        {
            var messageFrameBuilder = new MessageFrameBuilder(alloc, 238, FrameType.VariableShort);
            messageFrameBuilder.Put(MessageType.Int, Player.PingCount++ > 0xF42400 ? Player.PingCount = 1 : Player.PingCount);
            return messageFrameBuilder.ToMessageFrame();
        }
    }
}
