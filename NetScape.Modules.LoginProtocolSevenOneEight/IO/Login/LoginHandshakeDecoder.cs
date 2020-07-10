using System;
using ASPNetScape.Abstractions;
using ASPNetScape.Abstractions.Extensions;
using ASPNetScape.Abstractions.Interfaces.IO;
using ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.JS5;
using ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Model;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Login
{
    public class LoginHandshakeDecoder : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ILogger _logger;
        private readonly IJS5PacketSender _jS5PacketSender;

        public LoginHandshakeDecoder(ILogger logger, IJS5PacketSender jS5PacketSender)
        {
            _logger = logger;
            this._jS5PacketSender = jS5PacketSender;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer buffer)
        {
            int id = buffer.ReadByte();

            switch (id)
            {
                case (int)HandshakeRequest.LOGIN:
                    ctx.Channel.Pipeline.AddLast(nameof(LoginDecoder), new LoginDecoder(_logger));
                    ctx.FireChannelRead(buffer);
                    break;

                case (int)HandshakeRequest.JS_5:
                    DecodeJs5Header(ctx, buffer);
                    break;

                default:
                    _logger.Information($"Unexpected handshake request received: {id}");
                    return;
            }

            ctx.Channel.Pipeline.Remove(this);
        }

        private void DecodeJs5Header(IChannelHandlerContext session, IByteBuffer stream)
        {
            int size = stream.ReadByte();
            if (stream.ReadableBytes < size)
            {
                session.Channel.CloseAsync();
                return;
            }

            var clientBuild = stream.ReadInt();
            var customClientBuild = stream.ReadInt();

            if (!stream.ReadString().Equals(Constants.Js5Token))
            {
                session.Channel.CloseAsync();
                return;
            }

            _jS5PacketSender.SendStartup(session);
            session.Channel.Pipeline.AddLast(nameof(JS5Decoder), new JS5Decoder(_logger, _jS5PacketSender));
            session.Channel.Pipeline.AddFirst(nameof(JS5Encoder), new JS5Encoder());
        }
    }
}
