using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.Osrs.LoginProtocol
{
    public class HandshakeDecoder : ByteToMessageDecoder
    {
        private const int JS5_Handshake = 15;
        private const int Login_Handshake = 14;
        private readonly JS5Decoder _jS5Decoder;
        private readonly JS5Encoder _jS5Encoder;

        public HandshakeDecoder(JS5Decoder jS5Decoder, JS5Encoder jS5Encoder)
        {
            _jS5Decoder = jS5Decoder;
            _jS5Encoder = jS5Encoder;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            int handshakeOpcode = input.ReadByte();
            switch (handshakeOpcode)
            {
                case JS5_Handshake:
                    int revision = input.ReadInt();
                    Log.Logger.Information("JS5 Connection: {0} Revision: {1}", context.Channel.RemoteAddress, revision);
                    _ = context.Channel.WriteAndFlushAsync(context.Allocator.Buffer(1).WriteByte((int)LoginStatus.StatusExchangeData));
                    context.Channel.Pipeline.AddLast(nameof(JS5Decoder), _jS5Decoder);
                    context.Channel.Pipeline.AddLast(nameof(JS5Encoder), _jS5Encoder);
                    break;
                case Login_Handshake:
                    Log.Logger.Information("Login Connection: {0}", context.Channel.RemoteAddress);
                    break;
                default:
                    _ = context.DisconnectAsync();
                    break;
            }
            context.Channel.Pipeline.Remove(this);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception e)
        {
            Serilog.Log.Logger.Error(e, nameof(Decode));
            _ = context.CloseAsync();
        }
    }
}
