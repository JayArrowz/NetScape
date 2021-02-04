using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;

namespace NetScape.Modules.Messages.Decoders
{
    public abstract class MessageDecoderBase<TMessage> : IMessageDecoder<TMessage> where TMessage : DecoderMessage
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

        public abstract int[] Ids { get; }
        public abstract FrameType FrameType { get; }
        protected abstract TMessage Decode(Player player, MessageFrame frame);
        private readonly List<IObserver<TMessage>> _observers = new();

        public void DecodeAndPublish(Player player, MessageFrame frame)
        {
            var decoded = Decode(player, frame);
            decoded.Player = player;
            _observers.ForEach(t => t.OnNext(decoded));
        }

        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            _observers.Add(observer);
            return new MessageObserverDisposable(observer, _observers);
        }
    }
}
