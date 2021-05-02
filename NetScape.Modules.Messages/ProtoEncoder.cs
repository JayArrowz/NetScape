using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Model.Messages;
using NetScape.Modules.Messages.Builder;
using System;
using System.Collections.Generic;

namespace NetScape.Modules.Messages
{
    public class ProtoEncoder : MessageToMessageEncoder<ProtoMessage>
    {
        private readonly ProtoMessageCodecHandler _protoMessageCodecHandler;
        public ProtoEncoder(ProtoMessageCodecHandler protoMessageCodecHandler)
        {
            _protoMessageCodecHandler = protoMessageCodecHandler;
        }

        protected override void Encode(IChannelHandlerContext context, ProtoMessage message, List<object> output)
        {
            var protoMessageCodec = _protoMessageCodecHandler.EncoderCodecs[message.Opcode];
            var messageCodec = protoMessageCodec.MessageCodec;
            var fieldCodecs = protoMessageCodec.FieldCodec;
            Serilog.Log.Logger.Debug("Encoder Sent: {0} - {1} to {2}", message.Message, message.Message.Descriptor.ClrType.Name, message.Player.Username);
            var frameType = messageCodec.SizeType.GetFrameType();
            var bldr = new MessageFrameBuilder(context.Allocator, message.Opcode, frameType);
            foreach (var field in fieldCodecs)
            {
                var isString = field.FieldCodec.Type == Models.FieldType.String;
                MessageType? messageType = isString ? null : field.FieldCodec.Type.GetMessageType();
                var dataTransform = field.FieldCodec.Transform.GetDataTransformation();
                var dataOrder = field.FieldCodec.Order.GetDataOrder();
                object value = field.FieldDescriptor.Accessor.GetValue(message.Message);

                if (!isString)
                {
                    long unboxedInt = field.ToUnboxedNumber(value);
                    bldr.Put(messageType.Value, dataOrder, dataTransform, unboxedInt);
                } else
                {
                    bldr.PutString((string)value);
                }
            }
            output.Add(bldr.ToMessageFrame());
        }
    }
}
