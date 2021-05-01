using Dawn;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace NetScape.Modules.Messages.Decoders
{
    public class MessageDecoderBase<TMessage> : IMessageDecoder<TMessage> where TMessage : class
    {
        private class MessageObserverDisposable : IDisposable
        {
            private readonly List<IObserver<TMessage>> _observers;
            private readonly IObserver<TMessage> _observer;
            private bool _disposed;

            public MessageObserverDisposable(IObserver<TMessage> observer, List<IObserver<TMessage>> observers)
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
            return null;
        }

        private readonly List<IObserver<TMessage>> _observers = new();

        public void Publish(object message)
        {
            var castedMessage = Guard.Argument(message).Cast<TMessage>();
            _observers.ForEach(t => t.OnNext(castedMessage));
        }

        public void DecodeAndPublish(Player player, MessageFrame frame)
        {
            var decoded = Decode(player, frame);
            //decoded.Player = player;
            Publish(decoded);
        }

        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            _observers.Add(observer);
            return new MessageObserverDisposable(observer, _observers);
        }

        public IDisposable SubscribeDelegate(Delegate method)
        {
            Action<TMessage> action = Guard.Argument(method).Cast<Action<TMessage>>();
            return Subscribe(Observer.Create(action));
        }
    }
}
