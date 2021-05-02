using Google.Protobuf.Reflection;
using NetScape.Modules.Messages.Builder;
using NetScape.Modules.Messages.Models;
using System;

namespace NetScape.Modules.Messages
{
    public static class ProtoMessageExtensions
    {
        public static object ToObject(this ProtoFieldCodec field, ulong value)
        {
            object unboxedValue = null;
            switch (field.FieldDescriptor.FieldType)
            {
                case Google.Protobuf.Reflection.FieldType.Bytes:
                    unboxedValue = (byte)value;
                    break;
                case Google.Protobuf.Reflection.FieldType.Int32:
                case Google.Protobuf.Reflection.FieldType.SInt32:
                case Google.Protobuf.Reflection.FieldType.UInt32:
                    unboxedValue = (int)value;
                    break;

                case Google.Protobuf.Reflection.FieldType.UInt64:
                case Google.Protobuf.Reflection.FieldType.SInt64:
                case Google.Protobuf.Reflection.FieldType.Int64:
                    unboxedValue = (long)value;
                    break;

                default:
                    throw new NotSupportedException($"Unsupported type {field.FieldDescriptor.FieldType}");
            }

            return unboxedValue;
        }

        public static long ToUnboxedNumber(this ProtoFieldCodec field, object value)
        {
            long unboxedInt = 0;
            switch (field.FieldDescriptor.FieldType)
            {
                case Google.Protobuf.Reflection.FieldType.Bool:
                    unboxedInt = ((bool)value) ? 1 : 0;
                    break;
                case Google.Protobuf.Reflection.FieldType.Int32:
                case Google.Protobuf.Reflection.FieldType.SInt32:
                case Google.Protobuf.Reflection.FieldType.UInt32:
                    unboxedInt = (int)value;
                    break;

                case Google.Protobuf.Reflection.FieldType.UInt64:
                case Google.Protobuf.Reflection.FieldType.SInt64:
                case Google.Protobuf.Reflection.FieldType.Int64:
                    unboxedInt = (long)value;
                    break;

                default:
                    throw new NotSupportedException($"Unsupported type {field.FieldDescriptor.FieldType}");
            }

            return unboxedInt;
        }

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

        public static int GetSize(this Models.FieldType fieldType)
        {
            switch (fieldType)
            {
                case Models.FieldType.Byte:
                    return 1;
                case Models.FieldType.Int:
                    return 4;
                case Models.FieldType.Long:
                    return 8;
                case Models.FieldType.Short:
                    return 2;
                case Models.FieldType.TriByte:
                    return 3;
            }
            return 0;
        }

        public static MessageType GetMessageType(this Models.FieldType fieldType)
        {
            switch (fieldType)
            {
                case Models.FieldType.Byte:
                    return MessageType.Byte;
                case Models.FieldType.Int:
                    return MessageType.Int;
                case Models.FieldType.Long:
                    return MessageType.Long;
                case Models.FieldType.Short:
                    return MessageType.Short;
                case Models.FieldType.TriByte:
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
