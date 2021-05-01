using Google.Protobuf;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public interface IMessageDecoder
    {
        int[] Ids { get; }
        void DecodeAndPublish(Player player, MessageFrame frame);
        void Publish(Player player, object message);
        FrameType FrameType { get; }
        string TypeName { get; }
        IDisposable SubscribeDelegate(Delegate method);
        IDisposable SubscribeDelegateAsync(Delegate method);
    }

    public interface IMessageDecoder<TMessage> : IMessageDecoder, IObservable<DecoderMessage<TMessage>> where TMessage : IMessage<TMessage>
    {
    }
}
