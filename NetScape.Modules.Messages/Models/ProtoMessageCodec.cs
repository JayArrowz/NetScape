using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace NetScape.Modules.Messages.Models
{
    public class ProtoMessageCodec
    {
        public MessageCodec MessageCodec { get; }
        public List<ProtoFieldCodec> FieldCodec { get; }
        public Func<IMessage> CreationMethod { get; }
        public ProtoMessageCodec(Func<IMessage> creationMethod, MessageCodec messageCodec, List<ProtoFieldCodec> fieldCodec)
        {
            CreationMethod = creationMethod;
            MessageCodec = messageCodec;
            FieldCodec = fieldCodec;
        }
    }
}
