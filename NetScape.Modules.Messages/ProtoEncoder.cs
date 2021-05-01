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
            Serilog.Log.Logger.Debug($"{message.Message} - {message.Message.Descriptor.ClrType.Name}");
            var frameType = messageCodec.SizeType.GetFrameType();
            var bldr = new MessageFrameBuilder(context.Allocator, message.Opcode, frameType);
            foreach (var field in fieldCodecs)
            {
                var messageType = field.FieldCodec.Type.GetMessageType();
                var dataTransform = field.FieldCodec.Transform.GetDataTransformation();
                var dataOrder = field.FieldCodec.Order.GetDataOrder();
                object value = field.FieldDescriptor.Accessor.GetValue(message.Message);
                long unboxedInt = field.ToUnboxedNumber(value);
                bldr.Put(messageType, dataOrder, dataTransform, unboxedInt);
            }
            output.Add(bldr.ToMessageFrame());
        }
    }
}
