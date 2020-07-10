using ASPNetScape.Abstractions.Interfaces.IO;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;
using System;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.JS5
{
    public sealed class JS5Decoder : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ILogger _logger;
        private readonly IJS5PacketSender _js5PacketSender;

        public JS5Decoder(ILogger logger, IJS5PacketSender js5PacketSender)
        {
            _logger = logger;
            _js5PacketSender = js5PacketSender;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            while (msg.ReadableBytes > 0)
            {
                int packetId = msg.ReadByte();

                if (packetId == 0 || packetId == 1)
                {
                    DecodeCacheRequest(ctx, packetId, msg);
                    return;
                }
                DecodeOtherPacket(ctx, packetId, msg);
            }
        }

        private void DecodeCacheRequest(IChannelHandlerContext ctx, int packetId, IByteBuffer msg)
        {
            bool priority = packetId == 1;
            int indexId = msg.ReadByte();
            int archiveId = msg.ReadInt();

            _logger.Verbose($"Decoding JS5 Packet Request Index: {indexId} Archive: {archiveId} Priority: {priority}");
            if (archiveId <= 0)
            {
                return;
            }

            _js5PacketSender.SendCacheArchive(ctx, indexId, archiveId, priority);
        }


        private void DecodeOtherPacket(IChannelHandlerContext ctx, int packetId, IByteBuffer msg)
        {
            _logger.Verbose($"Incoming JS5 Packet {packetId} for channel {ctx.Channel.RemoteAddress}");
            if (packetId == 7)
            {
                ctx.Channel.CloseAsync();
                return;
            }
            if (packetId == 4)
            {
                var encryptionVal = msg.ReadByte();
                ctx.Channel.GetAttribute(JS5Encoder.JS5_Encoder_Key).Set(new IntWrapper { Value = encryptionVal });
                _logger.Verbose($"Set JS5 Encryption Value {encryptionVal} for channel {ctx.Channel.RemoteAddress}");

                if (msg.ReadUnsignedShort() != 0)
                {
                    ctx.Channel.CloseAsync();
                }
            }
            else
            {
                msg.SkipBytes(11);
            }
        }
    }
}
