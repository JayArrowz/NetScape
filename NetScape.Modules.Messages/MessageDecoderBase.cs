using Dawn;
using Google.Protobuf;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NetScape.Modules.Messages
{
    public class MessageDecoderBase<TMessage> : IMessageDecoder<TMessage> where TMessage : IMessage<TMessage>
    {
        private class MessageObserverDisposable : IDisposable
        {
            private readonly List<KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>>> _observers;
            private readonly KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>> _observer;
            private bool _disposed;

            public MessageObserverDisposable(KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>> observer, List<KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>>> observers)
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

        private readonly List<KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>>> _observers = new();

        public void Publish(Player player, object message)
        {
            var castedMessage = Guard.Argument(message).Cast<TMessage>();
            var decodedMessage = new DecoderMessage<TMessage>
            {
                Message = castedMessage,
                Player = player
            };
            for(int i = 0; i < _observers.Count; i++)
            {
                var observerDetails = _observers[i];
                var observer = observerDetails.Key;
                var filter = observerDetails.Value;
                var canRun = filter != null ? filter.Invoke(decodedMessage) : true;
                if(canRun)
                {
                    observer.OnNext(decodedMessage);
                }
            }
        }

        public void DecodeAndPublish(Player player, MessageFrame frame)
        {
            var decoded = Decode(player, frame);
            //decoded.Player = player;
            Publish(player, decoded);
        }

        private IDisposable Subscribe(IObserver<DecoderMessage<TMessage>> observer, Predicate<DecoderMessage<TMessage>> filter)
        {
            var keyValuePair = new KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>>(observer, filter);
            _observers.Add(keyValuePair);
            return new MessageObserverDisposable(keyValuePair, _observers);
        }

        public IDisposable SubscribeDelegate(Delegate method, Delegate filter)
        {
            Action<DecoderMessage<TMessage>> action = Guard.Argument(method).Cast<Action<DecoderMessage<TMessage>>>();
            Predicate<DecoderMessage<TMessage>> filterPredicate = filter == null ? null : Guard.Argument(filter).Cast<Predicate<DecoderMessage<TMessage>>>();
            return Subscribe(Observer.Create(action), filterPredicate);
        }

        public IDisposable SubscribeDelegateAsync(Delegate method, Delegate filter)
        {
            Func<DecoderMessage<TMessage>, Task> action = Guard.Argument(method).Cast<Func<DecoderMessage<TMessage>, Task>>();
            Predicate<DecoderMessage<TMessage>> filterPredicate = filter == null ? null : Guard.Argument(filter).Cast<Predicate<DecoderMessage<TMessage>>>();
            
            return Subscribe(Observer.Create<DecoderMessage<TMessage>>(async (e) => await action(e)), filterPredicate);
        }

        public IDisposable Subscribe(IObserver<DecoderMessage<TMessage>> observer)
        {
            return new MessageObserverDisposable(new KeyValuePair<IObserver<DecoderMessage<TMessage>>, Predicate<DecoderMessage<TMessage>>>(observer, null), _observers);
        }
    }
}
