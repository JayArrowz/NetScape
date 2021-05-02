﻿using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetScape.Modules.Messages
{
    public class MessageHeaderDecoder : ByteToMessageDecoder, ICipherAwareHandler, IPlayerAwareHandler
    {
        private readonly IMessageDecoder[] _messageDecoders;
        private readonly ProtoMessageCodecHandler _protoMessageCodecHandler;

        public MessageHeaderDecoder(IMessageDecoder[] messageDecoders, ProtoMessageCodecHandler protoMessageCodecHandler)
        {
            _messageDecoders = messageDecoders;
            _protoMessageCodecHandler = protoMessageCodecHandler;
        }

        public IsaacRandomPair CipherPair { get; set; }
        public Player Player { get; set; }

        public static readonly int[] PacketLengths = {
            0, 0, 0, 1, -1, 0, 0, 0, 0, 0, // 0
			0, 0, 0, 0, 8, 0, 6, 2, 2, 0, // 10
			0, 2, 0, 6, 0, 12, 0, 0, 0, 0, // 20
			0, 0, 0, 0, 0, 8, 4, 0, 0, 2, // 30
			2, 6, 0, 6, 0, -1, 0, 0, 0, 0, // 40
			0, 0, 0, 12, 0, 0, 0, 8, 8, 0, // 50
			0, 8, 0, 0, 0, 0, 0, 0, 0, 0, // 60
			6, 0, 2, 2, 8, 6, 0, -1, 0, 6, // 70
			0, 0, 0, 0, 0, 1, 4, 6, 0, 0, // 80
			0, 0, 0, 0, 0, 3, 0, 0, -1, 0, // 90
			0, 13, 0, -1, 0, 0, 0, 0, 0, 0, // 100
			0, 0, 0, 0, 0, 0, 0, 6, 0, 0, // 110
			1, 0, 6, 0, 0, 0, -1, 0, 2, 6, // 120
			0, 4, 6, 8, 0, 6, 0, 0, 0, 2, // 130
			0, 0, 0, 0, 0, 6, 0, 0, 0, 0, // 140
			0, 0, 1, 2, 0, 2, 6, 0, 0, 0, // 150
			0, 0, 0, 0, -1, -1, 0, 0, 0, 0, // 160
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 170
			0, 8, 0, 3, 0, 2, 0, 0, 8, 1, // 180
			0, 0, 12, 0, 0, 0, 0, 0, 0, 0, // 190
			2, 0, 0, 0, 0, 0, 0, 0, 4, 0, // 200
			4, 0, 0, 0, 7, 8, 0, 0, 10, 0, // 210
			0, 0, 0, 0, 0, 0, -1, 0, 6, 0, // 220
			1, 0, 0, 0, 6, 0, 6, 8, 1, 0, // 230
			0, 4, 0, 0, 0, 0, -1, 0, -1, 4, // 240
			0, 0, 6, 6, 0, 0, // 250
        };

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var isaacValue = CipherPair.DecodingRandom.NextInt();
            int opcode = input.ReadByte();
            int unencodedOpcode = opcode - isaacValue & 0xFF;

            var protoCodec = _protoMessageCodecHandler.DecoderCodecs.ContainsKey(unencodedOpcode) ?
                _protoMessageCodecHandler.DecoderCodecs[unencodedOpcode] : null;
            var decoder = _messageDecoders
                .Where(t => t.Ids != null)
                .FirstOrDefault(decoder => decoder.Ids.Contains(unencodedOpcode));
            var frameType = protoCodec == null ? decoder?.FrameType : protoCodec.MessageCodec.SizeType.GetFrameType();
            var size = 0;
            if (!frameType.HasValue)
            {
                Log.Logger.Warning("Opcode {0} not recognised, sent by player {1}", unencodedOpcode, Player.Username);
                _ = context.CloseAsync();
                return;
            }

            if (frameType != FrameType.Fixed)
            {
                size = frameType.Value.GetBytes(input);
            }
            else
            {
                size = protoCodec.FieldCodec.Sum(t => t.FieldCodec.Type.GetSize());
            }
            var buffer = input.ReadBytes(size);
            var messageFrame = new MessageFrame(unencodedOpcode, frameType.Value, buffer);
            if (protoCodec?.MessageCodec?.Custom ?? true)
            {
                Log.Logger.Debug("Decoding Opcode: {0} Player Name: {1} from {2} Size {3}", unencodedOpcode, Player.Username, context.Channel.RemoteAddress, size);
                decoder.DecodeAndPublish(Player, messageFrame);
            }
            else
            {
                var message = protoCodec.CreationMethod.Invoke();
                var messageReader = new MessageFrameReader(messageFrame);
                var protoDecoder = _messageDecoders.First(t => t.TypeName == message.Descriptor.ClrType.Name);

                foreach (var field in protoCodec.FieldCodec)
                {
                    var fieldType = field.FieldDescriptor.FieldType;
                    var isString = field.FieldCodec.Type == Models.FieldType.String;
                    MessageType? messageType = isString ? null : field.FieldCodec.Type.GetMessageType();
                    var order = field.FieldCodec.Order.GetDataOrder();
                    var transform = field.FieldCodec.Transform.GetDataTransformation();

                    object rawValue = isString ? messageReader.ReadString() : messageReader.GetUnsigned(messageType.Value, order, transform);
                    object value = fieldType == Google.Protobuf.Reflection.FieldType.Bool ? ((ulong)rawValue == 1)
                        : isString ? (string)rawValue
                        : field.ToObject((ulong)rawValue);
                    try
                    {
                        field.FieldDescriptor.Accessor.SetValue(message, value);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error("Error decoding field {0}, Error: {1}", field.FieldCodec, e);
                    }
                }

                Log.Logger.Debug("Message Recieved: {0} TypeName: {1} Player: {2}", message, protoDecoder.TypeName, Player.Username);
                protoDecoder.Publish(Player, message);
            }
        }
    }
}
