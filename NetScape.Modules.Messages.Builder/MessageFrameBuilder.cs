using DotNetty.Buffers;
using System;

namespace NetScape.Modules.Messages.Builder
{
    /**
     * @author Graham
     * Modified by JayArrowz
     */
    public class MessageFrameBuilder
    {
        private static readonly int[] BITMASKS = {
            0x0, 0x1, 0x3, 0x7,
            0xf, 0x1f, 0x3f, 0x7f,
            0xff, 0x1ff, 0x3ff, 0x7ff,
            0xfff, 0x1fff, 0x3fff, 0x7fff,
            0xffff, 0x1ffff, 0x3ffff, 0x7ffff,
            0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
            0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff,
            0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff,
            -1 };

        private int OpCode { get; }
        private FrameType Type { get; }
        public IByteBufferAllocator Alloc { get; }
        private IByteBuffer Buffer { get; }
        private AccessMode Mode { get; set; } = AccessMode.Byte;

        private int bitIndex;

        public MessageFrameBuilder(IByteBufferAllocator alloc) : this(alloc, -1, FrameType.Raw)
        {
        }

        public MessageFrameBuilder(IByteBufferAllocator alloc, int opcode) : this(alloc, opcode, FrameType.Fixed)
        {
        }

        public MessageFrameBuilder(IByteBufferAllocator alloc, int opcode, FrameType type)
        {
            Alloc = alloc;
            Buffer = alloc.Buffer();
            OpCode = opcode;
            Type = type;
        }

        public MessageFrame ToMessageFrame()
        {
            if (Type == FrameType.Raw)
                throw new ArgumentException("Raw builders cannot be converted to frames");

            if (Mode != AccessMode.Byte)
                throw new ArgumentException("Must be in byte access mode to convert to a packet");

            return new MessageFrame(OpCode, Type, Buffer);
        }

        public int GetLength()
        {
            CheckByteAccess();
            return Buffer.WriterIndex;
        }

        public void SwitchToByteAccess()
        {
            if (Mode == AccessMode.Byte)
            {
                throw new ArgumentException("Already in byte access mode");
            }
            Mode = AccessMode.Byte;
            Buffer.SetWriterIndex((bitIndex + 7) / 8);
        }

        public void SwitchToBitAccess()
        {
            if (Mode == AccessMode.Bit)
            {
                throw new ArgumentException("Already in bit access mode");
            }
            Mode = AccessMode.Bit;
            bitIndex = Buffer.WriterIndex * 8;
        }

        public void PutRawBuilder(MessageFrameBuilder builder)
        {
            CheckByteAccess();
            if (builder.Type != FrameType.Raw)
            {
                throw new ArgumentException("Builder must be raw!");
            }
            builder.CheckByteAccess();
            PutBytes(builder.Buffer);
        }

        /**
		 * Puts a raw builder in reverse. Both builders (this and parameter) must
		 * be in byte access mode.
		 * @param builder The builder.
		 */
        public void PutRawBuilderReverse(MessageFrameBuilder builder)
        {
            CheckByteAccess();
            if (builder.Type != FrameType.Raw)
            {
                throw new ArgumentException("Builder must be raw!");
            }
            builder.CheckByteAccess();
            PutBytesReverse(builder.Buffer);
        }

        /**
		 * Puts a standard data type with the specified value.
		 * @param type The data type.
		 * @param value The value.
		 * @throws IllegalStateException if this reader is not in byte access mode.
		 */
        public void Put(MessageType type, long value)
        {
            Put(type, DataOrder.Big, DataTransformation.None, value);
        }

        /**
		 * Puts a standard data type with the specified value and byte order.
		 * @param type The data type.
		 * @param order The byte order.
		 * @param value The value.
		 * @throws IllegalStateException if this reader is not in byte access mode.
		 * @throws ArgumentException if the combination is invalid.
		 */
        public void Put(MessageType type, DataOrder order, long value)
        {
            Put(type, order, DataTransformation.None, value);
        }

        /**
		 * Puts a standard data type with the specified value and transformation.
		 * @param type The type.
		 * @param transformation The transformation.
		 * @param value The value.
		 * @throws IllegalStateException if this reader is not in byte access mode.
		 * @throws ArgumentException if the combination is invalid.
		 */
        public void Put(MessageType type, DataTransformation transformation, long value)
        {
            Put(type, DataOrder.Big, transformation, value);
        }

        /**
		 * Puts a standard data type with the specified value, byte order and
		 * transformation.
		 * @param type The data type.
		 * @param order The byte order.
		 * @param transformation The transformation.
		 * @param value The value.
		 * @throws IllegalStateException if this reader is not in byte access mode.
		 * @throws ArgumentException if the combination is invalid.
		 */
        public void Put(MessageType type, DataOrder order, DataTransformation transformation, long value)
        {
            CheckByteAccess();
            int length = (int)type;
            if (order == DataOrder.Big)
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    if (i == 0 && transformation != DataTransformation.None)
                    {
                        if (transformation == DataTransformation.Add)
                        {
                            Buffer.WriteByte((byte)(value + 128));
                        }
                        else if (transformation == DataTransformation.Negate)
                        {
                            Buffer.WriteByte((byte)-value);
                        }
                        else if (transformation == DataTransformation.Subtract)
                        {
                            Buffer.WriteByte((byte)(128 - value));
                        }
                        else
                        {
                            throw new ArgumentException("unknown transformation");
                        }
                    }
                    else
                    {
                        Buffer.WriteByte((byte)(value >> i * 8));
                    }
                }
            }
            else if (order == DataOrder.Little)
            {
                for (int i = 0; i < length; i++)
                {
                    if (i == 0 && transformation != DataTransformation.None)
                    {
                        if (transformation == DataTransformation.Add)
                        {
                            Buffer.WriteByte((byte)(value + 128));
                        }
                        else if (transformation == DataTransformation.Negate)
                        {
                            Buffer.WriteByte((byte)-value);
                        }
                        else if (transformation == DataTransformation.Subtract)
                        {
                            Buffer.WriteByte((byte)(128 - value));
                        }
                        else
                        {
                            throw new ArgumentException("unknown transformation");
                        }
                    }
                    else
                    {
                        Buffer.WriteByte((byte)(value >> i * 8));
                    }
                }
            }
            else if (order == DataOrder.Middle)
            {
                if (transformation != DataTransformation.None)
                {
                    throw new ArgumentException("middle endian cannot be transformed");
                }
                if (type != MessageType.Int)
                {
                    throw new ArgumentException("middle endian can only be used with an integer");
                }
                Buffer.WriteByte((byte)(value >> 8));
                Buffer.WriteByte((byte)value);
                Buffer.WriteByte((byte)(value >> 24));
                Buffer.WriteByte((byte)(value >> 16));
            }
            else if (order == DataOrder.InversedMiddle)
            {
                if (transformation != DataTransformation.None)
                {
                    throw new ArgumentException("inversed middle endian cannot be transformed");
                }
                if (type != MessageType.Int)
                {
                    throw new ArgumentException("inversed middle endian can only be used with an integer");
                }
                Buffer.WriteByte((byte)(value >> 16));
                Buffer.WriteByte((byte)(value >> 24));
                Buffer.WriteByte((byte)value);
                Buffer.WriteByte((byte)(value >> 8));
            }
            else
            {
                throw new ArgumentException("unknown order");
            }
        }

        /**
		 * Puts a string into the buffer.
		 * @param str The string.
		 */
        public void PutString(string str)
        {
            CheckByteAccess();
            char[] chars = str.ToCharArray();
            foreach (char c in chars)
            {
                Buffer.WriteByte((byte)c);
            }
            Buffer.WriteByte(0);
        }

        /**
		 * Puts a smart into the buffer.
		 * @param value The value.
		 */
        public void PutSmart(int value)
        {
            CheckByteAccess();
            if (value < 128)
            {
                Buffer.WriteByte(value);
            }
            else
            {
                Buffer.WriteShort(value);
            }
        }

        /**
		 * Puts the bytes from the specified buffer into this packet's buffer.
		 * @param buffer The source {@link ByteBuf}.
		 * @throws IllegalStateException if the builder is not in byte access mode.
		 */
        public void PutBytes(IByteBuffer buffer)
        {
            byte[] bytes = new byte[buffer.ReadableBytes];
            buffer.MarkReaderIndex();
            try
            {
                buffer.ReadBytes(bytes);
            }
            finally
            {
                buffer.ResetReaderIndex();
            }
            PutBytes(bytes);
        }

        /**
		 * Puts the bytes from the specified buffer into this packet's buffer, in
		 * reverse.
		 * @param buffer The source {@link ByteBuf}.
		 * @throws IllegalStateException if the builder is not in byte access mode.
		 */
        public void PutBytesReverse(IByteBuffer buffer)
        {
            byte[] bytes = new byte[buffer.ReadableBytes];
            buffer.MarkReaderIndex();
            try
            {
                buffer.ReadBytes(bytes);
            }
            finally
            {
                buffer.ResetReaderIndex();
            }
            PutBytesReverse(bytes);
        }

        /**
		 * Puts the specified byte array into the buffer.
		 * @param bytes The byte array.
		 * @throws IllegalStateException if the builder is not in bit access mode.
		 */
        public void PutBytes(byte[] bytes)
        {
            Buffer.WriteBytes(bytes);
        }

        /**
		 * Puts the bytes into the buffer with the specified transformation.
		 * @param transformation The transformation.
		 * @param bytes The byte array.
		 * @throws IllegalStateException if the builder is not in byte access mode.
		 */
        public void PutBytes(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
            {
                PutBytes(bytes);
            }
            else
            {
                foreach (byte b in bytes)
                {
                    Put(MessageType.Byte, transformation, b);
                }
            }
        }

        /**
		 * Puts the specified byte array into the buffer in reverse.
		 * @param bytes The byte array.
		 * @throws IllegalStateException if the builder is not in byte access mode.
		 */
        public void PutBytesReverse(byte[] bytes)
        {
            CheckByteAccess();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                Buffer.WriteByte(bytes[i]);
            }
        }

        /**
		 * Puts the specified byte array into the buffer in reverse with the
		 * specified transformation.
		 * @param transformation The transformation.
		 * @param bytes The byte array.
		 * @throws IllegalStateException if the builder is not in byte access mode.
		 */
        public void PutBytesReverse(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
            {
                PutBytesReverse(bytes);
            }
            else
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    Put(MessageType.Byte, transformation, bytes[i]);
                }
            }
        }

        /**
		 * Puts a single bit into the buffer. If {@code flag} is {@code true}, the
		 * value of the bit is {@code 1}. If {@code flag} is {@code false}, the
		 * value of the bit is {@code 0}.
		 * @param flag The flag.
		 * @throws IllegalStateException if the builder is not in bit access mode.
		 */
        public void PutBit(bool flag)
        {
            PutBit(flag ? 1 : 0);
        }

        /**
		 * Puts a single bit into the buffer with the value {@code value}.
		 * @param value The value.
		 * @throws IllegalStateException if the builder is not in bit access mode.
		 */
        public void PutBit(int value)
        {
            PutBits(1, value);
        }

        /**
		 * Puts {@code numBits} into the buffer with the value {@code value}.
		 * @param numBits The number of bits to put into the buffer.
		 * @param value The value.
		 * @throws IllegalStateException if the builder is not in bit access mode.
		 * @throws ArgumentException if the number of bits is not between 1
		 * and 31 inclusive.
		 */
        public void PutBits(int numBits, int value)
        {
            if (numBits <= 0 || numBits > 32)
            {
                throw new ArgumentException("Number of bits must be between 1 and 31 inclusive");
            }

            CheckBitAccess();

            int bytePos = bitIndex >> 3;
            int bitOffset = 8 - (bitIndex & 7);
            bitIndex += numBits;

            int requiredSpace = bytePos - Buffer.WriterIndex + 1;
            requiredSpace += (numBits + 7) / 8;
            Buffer.EnsureWritable(requiredSpace);

            for (; numBits > bitOffset; bitOffset = 8)
            {
                int tmp = Buffer.GetByte(bytePos);
                tmp &= ~BITMASKS[bitOffset];
                tmp |= value >> numBits - bitOffset & BITMASKS[bitOffset];
                Buffer.SetByte(bytePos++, tmp);
                numBits -= bitOffset;
            }
            if (numBits == bitOffset)
            {
                int tmp = Buffer.GetByte(bytePos);
                tmp &= ~BITMASKS[bitOffset];
                tmp |= value & BITMASKS[bitOffset];
                Buffer.SetByte(bytePos, tmp);
            }
            else
            {
                int tmp = Buffer.GetByte(bytePos);
                tmp &= ~(BITMASKS[numBits] << bitOffset - numBits);
                tmp |= (value & BITMASKS[numBits]) << bitOffset - numBits;
                Buffer.SetByte(bytePos, tmp);
            }
        }

        /**
		 * Checks that this builder is in the byte access mode.
		 * @throws IllegalStateException if the builder is not in byte access mode.
		 */
        private void CheckByteAccess()
        {
            if (Mode != AccessMode.Byte)
            {
                throw new ArgumentException("For byte-based calls to work, the mode must be byte access");
            }
        }

        /**
		 * Checks that this builder is in the bit access mode.
		 * @throws IllegalStateException if the builder is not in bit access mode.
		 */
        private void CheckBitAccess()
        {
            if (Mode != AccessMode.Bit)
            {
                throw new ArgumentException("For bit-based calls to work, the mode must be bit access");
            }
        }

    }

}
