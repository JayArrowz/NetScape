using NetScape.Modules.Messages.Builder;
using NetScape.Modules.Messages.Models;
using System;

namespace NetScape.Modules.Messages
{
    public static class ProtoMessageExtensions
    {
        public static FrameType GetFrameType(this FrameSizeType sizeType)
        {
            switch (sizeType)
            {
                case FrameSizeType.FixedByte:
                    return FrameType.Fixed;
                case FrameSizeType.Raw:
                    return FrameType.Raw;
                case FrameSizeType.VariableByte:
                    return FrameType.VariableByte;
                case FrameSizeType.VariableShort:
                    return FrameType.VariableShort;
            }

            throw new NotSupportedException($"FrameSizeType Not Supported {sizeType}");
        }

        public static DataOrder GetDataOrder(this FieldOrder order)
        {
            switch (order)
            {
                case FieldOrder.Big:
                    return DataOrder.Big;
                case FieldOrder.InversedMiddle:
                    return DataOrder.InversedMiddle;
                case FieldOrder.Little:
                    return DataOrder.Little;
                case FieldOrder.Middle:
                    return DataOrder.Middle;
            }

            throw new NotSupportedException($"FieldOrder Not Supported {order}");
        }

        public static MessageType GetMessageType(this FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Byte:
                    return MessageType.Byte;
                case FieldType.Int:
                    return MessageType.Int;
                case FieldType.Long:
                    return MessageType.Long;
                case FieldType.Short:
                    return MessageType.Short;
                case FieldType.TriByte:
                    return MessageType.TriByte;
            }

            throw new NotSupportedException($"FieldType Not Supported {fieldType}");
        }

        public static DataTransformation GetDataTransformation(this FieldTransform fieldTransform)
        {
            switch (fieldTransform)
            {
                case FieldTransform.Add:
                    return DataTransformation.Add;
                case FieldTransform.Negate:
                    return DataTransformation.Negate;
                case FieldTransform.None:
                    return DataTransformation.None;
                case FieldTransform.Subtract:
                    return DataTransformation.Subtract;
            }

            throw new NotSupportedException($"FieldTransform Not Supported {fieldTransform}");
        }
    }
}
