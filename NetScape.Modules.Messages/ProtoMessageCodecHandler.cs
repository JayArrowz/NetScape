using Autofac;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using NetScape.Modules.Messages.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NetScape.Modules.Messages
{
    public class ProtoMessageCodecHandler : IStartable
    {
        private readonly Type[] _codecTypes;
        public ProtoMessageCodecHandler(Type[] codecTypes)
        {
            _codecTypes = codecTypes;
        }

        public Dictionary<int, ProtoMessageCodec> EncoderCodecs { get; set; }
        public Dictionary<int, ProtoMessageCodec> DecoderCodecs { get; set; }

        public Dictionary<Type, int> EncoderTypeMap { get; set; }

        public static List<Type> GetExporedTypesForCodec(Type[] codecTypes)
        {
            return typeof(MessageCodec).Assembly.ExportedTypes.Where(t =>
                t.IsAssignableTo<IMessage>() && codecTypes.Contains(t.DeclaringType))
                .ToList();
        }

        public void Start()
        {
            EncoderCodecs = new();
            DecoderCodecs = new();
            EncoderTypeMap = new();
            GetExporedTypesForCodec(_codecTypes)
                .ForEach(t =>
                {
                    var descriptor = (MessageDescriptor)t.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static).GetValue(null, null); // get the static property Descriptor
                    var codecExists = descriptor.CustomOptions.TryGetMessage<MessageCodec>(2001, out var messageCodec);
                    if (codecExists)
                    {
                        descriptor.CustomOptions.TryGetBool(2002, out bool isEncoder);
                        messageCodec.OpCodes.ToList().ForEach(opcode =>
                        {
                            var creationMethod = (Func<IMessage>)Expression.Lambda(typeof(Func<IMessage>),
                                                 Expression.New(t)
                                                ).Compile();
                            var map = isEncoder ? EncoderCodecs : DecoderCodecs;
                            if (isEncoder)
                            {
                                EncoderTypeMap.Add(t, (int)opcode);
                            }

                            Log.Logger.Debug("Detected MessageCodec {0} - {1}", opcode, t);
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
