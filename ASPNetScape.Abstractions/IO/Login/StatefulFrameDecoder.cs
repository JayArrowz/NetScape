using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Abstractions.IO.Login
{
    /**
     * A stateful implementation of a {@link ByteToMessageDecoder} which may be extended and used by other classes. The
     * current state is tracked by this class and is a user-specified enumeration.
     *
     * The state may be changed by calling the {@link StatefulFrameDecoder#setState} method.
     *
     * The current state is supplied as a parameter in the {@link StatefulFrameDecoder#decode} and
     * {@link StatefulFrameDecoder#decodeLast} methods.
     *
     * This class is not thread safe: it is recommended that the state is only set in the decode methods overridden.
     *
     * @author Graham
     * @param <T> The state enumeration.
     */
    public abstract class StatefulFrameDecoder<T> : ByteToMessageDecoder
    {

        /**
         * The current state.
         */
        private T _state;

        /**
         * Creates the stateful frame decoder with the specified initial state.
         *
         * @param state The initial state.
         * @throws NullPointerException If the state is {@code null}.
         */
        public StatefulFrameDecoder(T state)
        {
            SetState(state);
        }

        /**
         * Sets a new state.
         *
         * @param state The new state.
         * @throws NullPointerException If the state is {@code null}.
         */
        public void SetState(T state)
        {
            this._state = state;
        }

        protected override void Decode(IChannelHandlerContext context,
            IByteBuffer input,
            List<object> output)
        {
            Decode(context, input, output, _state);
        }

        /**
         * Decodes the received packets into a frame.
         *
         * @param ctx The current context of this handler.
         * @param in The cumulative buffer, which may contain zero or more bytes.
         * @param out The {@link List} of objects to pass forward through the pipeline.
         * @param state The current state. The state may be changed by calling {@link #setState}.
         * @throws Exception If there is an exception when decoding a frame.
         */
        protected abstract void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output, T state);
    }
}
