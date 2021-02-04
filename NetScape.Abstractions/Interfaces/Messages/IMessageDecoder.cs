using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public interface IMessageDecoder
    {
        int[] Ids { get; }
        void DecodeAndPublish(Player player, MessageFrame frame);
        FrameType FrameType { get; }
    }

    public interface IMessageDecoder<TMessage> : IMessageDecoder, IObservable<TMessage>
    {
    }
}
