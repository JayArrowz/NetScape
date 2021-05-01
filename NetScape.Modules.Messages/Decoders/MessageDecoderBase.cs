using Dawn;
using Google.Protobuf;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace NetScape.Modules.Messages.Decoders
{
    public class MessageDecoderBase<TMessage> : IMessageDecoder<TMessage> where TMessage : IMessage<TMessage>
    {
        private class MessageObserverDisposable : IDisposable
        {
            private readonly List<IObserver<DecoderMessage<TMessage>>> _observers;
            private readonly IObserver<DecoderMessage<TMessage>> _observer;
            private bool _disposed;

            public MessageObserverDisposable(IObserver<DecoderMessage<TMessage>> observer, List<IObserver<DecoderMessage<TMessage>>> observers)
            {
                _observer = observer;
                _observers = observers;
            }
            public void Dispose()
            {
                if (!_disposed)
                {
                    _observers.Remove(_observer);
                    _disposed = true;
                }
            }
        }

        public virtual int[] Ids { get; }
        public virtual FrameType FrameType { get; }

        public string TypeName => typeof(TMessage).Name;

        protected virtual TMessage Decode(Player player, MessageFrame frame)
        {
            return default(TMessage);
        }

        private readonly List<IObserver<DecoderMessage<TMessage>>> _observers = new();

        public void Publish(Player player, object message)
        {
            var castedMessage = Guard.Argument(message).Cast<TMessage>();
            var decodedMessage = new DecoderMessage<TMessage>
            {
                Message = castedMessage,
                Player = player
            };
            _observers.ForEach(t => t.OnNext(decodedMessage));
        }

        public void DecodeAndPublish(Player player, MessageFrame frame)
        {
            var decoded = Decode(player, frame);
            //decoded.Player = player;
            Publish(player, decoded);
        }

        public IDisposable Subscribe(IObserver<DecoderMessage<TMessage>> observer)
        {
            _observers.Add(observer);
            return new MessageObserverDisposable(observer, _observers);
        }

        public IDisposable SubscribeDelegate(Delegate method)
        {
            Action<DecoderMessage<TMessage>> action = Guard.Argument(method).Cast<Action<DecoderMessage<TMessage>>>();
            return Subscribe(Observer.Create(action));
        }
    }
}
