using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using NetScape.Abstractions.Util;

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

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            int? isaacValue = CipherPair?.DecodingRandom?.NextInt() ?? null;
            int opcode = input.ReadByte();
            int unencodedOpcode =  isaacValue.HasValue ? opcode - isaacValue.Value & 0xFF : opcode & 0XFF;

            var protoCodec = _protoMessageCodecHandler.DecoderCodecs.ContainsKey(unencodedOpcode) ?
                _protoMessageCodecHandler.DecoderCodecs[unencodedOpcode] : null;
            var decoder = _messageDecoders
                .Where(t => t.Ids != null)
                .FirstOrDefault(decoder => decoder.Ids.Contains(unencodedOpcode));
            var frameType = protoCodec?.MessageCodec.SizeType.GetFrameType() ?? decoder?.FrameType;
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
                decoder?.DecodeAndPublish(Player, messageFrame);
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

                Log.Logger.Debug("Message Received: {0} TypeName: {1} Player: {2} Size: {3} Opcode: {4}", message, protoDecoder.TypeName, Player.Username, size, unencodedOpcode);
                protoDecoder.Publish(Player, message);
            }
        }
    }
}
