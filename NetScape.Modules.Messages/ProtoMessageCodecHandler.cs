using Autofac;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using NetScape.Modules.Messages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NetScape.Modules.Messages
{
    public class ProtoMessageCodecHandler : IStartable
    {
        public Dictionary<int, ProtoMessageCodec> EncoderCodecs { get; set; }
        public Dictionary<int, ProtoMessageCodec> DecoderCodecs { get; set; }

        public Dictionary<Type, int> EncoderTypeMap { get; set; }
        public void Start()
        {
            EncoderCodecs = new();
            DecoderCodecs = new();
            EncoderTypeMap = new();
            typeof(MessageCodec).Assembly.ExportedTypes.Where(t => t.IsAssignableTo<IMessage>())
                .ToList()
                .ForEach(t =>
                {
                    var descriptor = (MessageDescriptor)t.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static).GetValue(null, null); // get the static property Descriptor
                    var codecExists = descriptor.CustomOptions.TryGetMessage<MessageCodec>(2001, out var messageCodec);
                    if (codecExists)
                    {
                        descriptor.CustomOptions.TryGetBool(2002, out bool isEncoder);
                        messageCodec.OpCodes.ToList().ForEach(opcode =>
                        {
                            var creationMethod = (Func<IMessage>) Expression.Lambda(typeof(Func<IMessage>),
                                                 Expression.New(t)
                                                ).Compile();
                            var map = isEncoder ? EncoderCodecs : DecoderCodecs;
                            if(isEncoder)
                            {
                                EncoderTypeMap.Add(t, (int)opcode);
                            }

                            map.Add((int)opcode, new ProtoMessageCodec(creationMethod, messageCodec, descriptor
                                .Fields
                                .InFieldNumberOrder()
                                .Select(field =>
                                {
                                    field.CustomOptions.TryGetMessage<Models.FieldCodec>(2003, out var fieldCodec);
                                    return new ProtoFieldCodec(field, fieldCodec);
                                }).ToList()
                                ));
                        });
                    }
                });
        }
    }
}
