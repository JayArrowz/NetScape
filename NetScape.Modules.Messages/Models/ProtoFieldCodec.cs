using Google.Protobuf.Reflection;

namespace NetScape.Modules.Messages.Models
{
    public class ProtoFieldCodec
    {
        public FieldDescriptor FieldDescriptor { get; }
        public FieldCodec FieldCodec { get; }

        public ProtoFieldCodec(FieldDescriptor fieldDescriptor, FieldCodec fieldCodec)
        {
            FieldDescriptor = fieldDescriptor;
            FieldCodec = fieldCodec;
        }
    }
}