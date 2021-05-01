using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public interface IMessageDecoder
    {
        int[] Ids { get; }
        void DecodeAndPublish(Player player, MessageFrame frame);
        void Publish(object message);
        FrameType FrameType { get; }
        string TypeName { get; }
        IDisposable SubscribeDelegate(Delegate method);
    }

    public interface IMessageDecoder<TMessage> : IMessageDecoder, IObservable<TMessage>
    {
    }
}
