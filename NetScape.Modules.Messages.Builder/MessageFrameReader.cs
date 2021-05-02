using Dawn;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetScape.Modules.Messages.Builder
{
    public class MessageFrameReader
    {

        /**
		 * The current bit index.
		 */
        private int _bitIndex;

        /**
		 * The buffer.
		 */
        private readonly IByteBuffer _buffer;

        /**
         * The current mode.
         */
        private AccessMode _mode = AccessMode.Byte;

        /**
		 * Creates the reader.
		 *
		 * @param packet The packet.
		 */
        public MessageFrameReader(MessageFrame packet)
        {
            _buffer = packet.Payload;
        }

        /**
		 * Checks that this reader is in the bit access mode.
		 *
		 * @throws IllegalStateException If the reader is not in bit access mode.
		 */
        private void CheckBitAccess()
        {
            Guard.Argument(_mode).Equal(AccessMode.Bit);
        }

        /**
		 * Checks that this reader is in the byte access mode.
		 *
		 * @throws IllegalStateException If the reader is not in byte access mode.
		 */
        private void CheckByteAccess()
        {
            Guard.Argument(_mode).Equal(AccessMode.Byte);
        }

        /**
		 * Reads a standard data type from the buffer with the specified order and transformation.
		 *
		 * @param type The data type.
		 * @param order The data order.
		 * @param transformation The data transformation.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        private long Get(MessageType type, DataOrder order, DataTransformation transformation)
        {
            CheckByteAccess();
            long longValue = 0;
            int length = (int)type;
            if (order == DataOrder.Big)
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    if (i == 0 && transformation != DataTransformation.None)
                    {
                        if (transformation == DataTransformation.Add)
                        {
                            longValue |= _buffer.ReadByte() - 128 & 0xFFL;
                        }
                        else if (transformation == DataTransformation.Negate)
                        {
                            longValue |= -_buffer.ReadByte() & 0xFFL;
                        }
                        else if (transformation == DataTransformation.Subtract)
                        {
                            longValue |= 128 - _buffer.ReadByte() & 0xFFL;
                        }
                        else
                        {
                            throw new ArgumentException("Unknown transformation.");
                        }
                    }
                    else
                    {
                        longValue |= (_buffer.ReadByte() & 0xFFL) << i * 8;
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
                            longValue |= _buffer.ReadByte() - 128 & 0xFFL;
                        }
                        else if (transformation == DataTransformation.Negate)
                        {
                            longValue |= -_buffer.ReadByte() & 0xFFL;
                        }
                        else if (transformation == DataTransformation.Subtract)
                        {
                            longValue |= 128 - _buffer.ReadByte() & 0xFFL;
                        }
                        else
                        {
                            throw new ArgumentException("Unknown transformation.");
                        }
                    }
                    else
                    {
                        longValue |= (_buffer.ReadByte() & 0xFFL) << i * 8;
                    }
                }
            }
            else if (order == DataOrder.Middle)
            {
                if (transformation != DataTransformation.None)
                {
                    throw new ArgumentException("Middle endian cannot be transformed.");
                }
                if (type != MessageType.Int)
                {
                    throw new ArgumentException("Middle endian can only be used with an integer.");
                }
                longValue |= (_buffer.ReadByte() & 0xFF) << 8;
                longValue |= _buffer.ReadByte() & 0xFF;
                longValue |= (_buffer.ReadByte() & 0xFF) << 24;
                longValue |= (_buffer.ReadByte() & 0xFF) << 16;
            }
            else if (order == DataOrder.InversedMiddle)
            {
                if (transformation != DataTransformation.None)
                {
                    throw new ArgumentException("Inversed middle endian cannot be transformed.");
                }
                if (type != MessageType.Int)
                {
                    throw new ArgumentException("Inversed middle endian can only be used with an integer.");
                }
                longValue |= (_buffer.ReadByte() & 0xFF) << 16;
                longValue |= (_buffer.ReadByte() & 0xFF) << 24;
                longValue |= _buffer.ReadByte() & 0xFF;
                longValue |= (_buffer.ReadByte() & 0xFF) << 8;
            }
            else
            {
                throw new ArgumentException("Unknown order.");
            }
            return longValue;
        }

        /**
		 * Gets a bit from the buffer.
		 *
		 * @return The value.
		 * @throws IllegalStateException If the reader is not in bit access mode.
		 */
        public int GetBit()
        {
            return GetBits(1);
        }

        /**
		 * Gets the specified amount of bits from the buffer.
		 *
		 * @param amount The amount of bits.
		 * @return The value.
		 * @throws IllegalStateException If the reader is not in bit access mode.
		 * @throws IllegalArgumentException If the number of bits is not between 1 and 31 inclusive.
		 */
        public int GetBits(int amount)
        {
            Guard.Argument(amount).InRange(0, 32);
            CheckBitAccess();

            int bytePos = _bitIndex >> 3;
            int bitOffset = 8 - (_bitIndex & 7);
            int value = 0;
            _bitIndex += amount;

            for (; amount > bitOffset; bitOffset = 8)
            {
                value += (_buffer.GetByte(bytePos++) & MessageFrameBuilder.BITMASKS[bitOffset]) << amount - bitOffset;
                amount -= bitOffset;
            }
            if (amount == bitOffset)
            {
                value += _buffer.GetByte(bytePos) & MessageFrameBuilder.BITMASKS[bitOffset];
            }
            else
            {
                value += _buffer.GetByte(bytePos) >> bitOffset - amount & MessageFrameBuilder.BITMASKS[amount];
            }
            return value;
        }

        /**
		 * Gets bytes.
		 *
		 * @param bytes The target byte array.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public void GetBytes(byte[] bytes)
        {
            CheckByteAccess();
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = _buffer.ReadByte();
            }
        }

        /**
		 * Gets bytes with the specified transformation.
		 *
		 * @param transformation The transformation.
		 * @param bytes The target byte array.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public void GetBytes(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
            {
                GetBytesReverse(bytes);
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (byte)GetSigned(MessageType.Byte, transformation);
                }
            }
        }

        /**
		 * Gets bytes in reverse.
		 *
		 * @param bytes The target byte array.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public void GetBytesReverse(byte[] bytes)
        {
            CheckByteAccess();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                bytes[i] = _buffer.ReadByte();
            }
        }

        /**
		 * Gets bytes in reverse with the specified transformation.
		 *
		 * @param transformation The transformation.
		 * @param bytes The target byte array.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public void GetBytesReverse(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
            {
                GetBytesReverse(bytes);
            }
            else
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    bytes[i] = (byte)GetSigned(MessageType.Byte, transformation);
                }
            }
        }

        /**
		 * Gets the length of this reader.
		 *
		 * @return The length of this reader.
		 */
        public int GetLength()
        {
            CheckByteAccess();
            return _buffer.WritableBytes;
        }

        /**
		 * Gets a signed data type from the buffer.
		 *
		 * @param type The data type.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public long GetSigned(MessageType type)
        {
            return GetSigned(type, DataOrder.Big, DataTransformation.None);
        }

        /**
		 * Gets a signed data type from the buffer with the specified order.
		 *
		 * @param type The data type.
		 * @param order The byte order.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        public long GetSigned(MessageType type, DataOrder order)
        {
            return GetSigned(type, order, DataTransformation.None);
        }

        /**
		 * Gets a signed data type from the buffer with the specified order and transformation.
		 *
		 * @param type The data type.
		 * @param order The byte order.
		 * @param transformation The data transformation.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        public long GetSigned(MessageType type, DataOrder order, DataTransformation transformation)
        {
            long longValue = Get(type, order, transformation);
            if (type != MessageType.Long)
            {
                int max = (int)(Math.Pow(2, (int)type * 8 - 1) - 1);
                if (longValue > max)
                {
                    longValue -= (max + 1) * 2;
                }
            }
            return longValue;
        }

        /**
		 * Gets a signed data type from the buffer with the specified transformation.
		 *
		 * @param type The data type.
		 * @param transformation The data transformation.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        public long GetSigned(MessageType type, DataTransformation transformation)
        {
            return GetSigned(type, DataOrder.Big, transformation);
        }

        /**
		 * Gets a signed smart from the buffer.
		 *
		 * @return The smart.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public int GetSignedSmart()
        {
            CheckByteAccess();
            int peek = _buffer.GetByte(_buffer.ReaderIndex);
            if (peek < 128)
            {
                return _buffer.ReadByte() - 64;
            }
            return _buffer.ReadShort() - 49152;
        }

        /**
		 * Gets an unsigned data type from the buffer.
		 *
		 * @param type The data type.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public ulong GetUnsigned(MessageType type)
        {
            return GetUnsigned(type, DataOrder.Big, DataTransformation.None);
        }

        /**
		 * Gets an unsigned data type from the buffer with the specified order.
		 *
		 * @param type The data type.
		 * @param order The byte order.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        public ulong GetUnsigned(MessageType type, DataOrder order)
        {
            return GetUnsigned(type, order, DataTransformation.None);
        }

        /**
		 * Gets an unsigned data type from the buffer with the specified order and transformation.
		 *
		 * @param type The data type.
		 * @param order The byte order.
		 * @param transformation The data transformation.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        public ulong GetUnsigned(MessageType type, DataOrder order, DataTransformation transformation)
        {
            long longValue = Get(type, order, transformation);
            return ((ulong)longValue) & 0xFFFFFFFFFFFFFFFFL;
        }

        /**
		 * Gets an unsigned data type from the buffer with the specified transformation.
		 *
		 * @param type The data type.
		 * @param transformation The data transformation.
		 * @return The value.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 * @throws IllegalArgumentException If the combination is invalid.
		 */
        public ulong GetUnsigned(MessageType type, DataTransformation transformation)
        {
            return GetUnsigned(type, DataOrder.Big, transformation);
        }

        /**
		 * Gets an unsigned smart from the buffer.
		 *
		 * @return The smart.
		 * @throws IllegalStateException If this reader is not in byte access mode.
		 */
        public int GetUnsignedSmart()
        {
            CheckByteAccess();
            int peek = _buffer.GetByte(_buffer.ReaderIndex);
            if (peek < 128)
            {
                return _buffer.ReadByte();
            }
            return _buffer.ReadShort() - 32768;
        }

        /**
		 * Switches this builder's mode to the bit access mode.
		 *
		 * @throws IllegalStateException If the builder is already in bit access mode.
		 */
        public void SwitchToBitAccess()
        {
            Guard.Argument(_mode).Equal(AccessMode.Byte);
            _mode = AccessMode.Bit;
            _bitIndex = _buffer.ReaderIndex * 8;
        }

        /**
		 * Switches this builder's mode to the byte access mode.
		 *
		 * @throws IllegalStateException If the builder is already in byte access mode.
		 */
        public void SwitchToByteAccess()
        {
            Guard.Argument(_mode).Equal(AccessMode.Bit);
            _mode = AccessMode.Byte;
            _buffer.SetReaderIndex((_bitIndex + 7) / 8);
        }

        public string ReadString()
        {
            Guard.Argument(_mode).Equal(AccessMode.Byte);
            var strBldr = new StringBuilder();
            int charByte;
            while ((charByte = _buffer.ReadByte()) != 10)
            {
                strBldr.Append((char)charByte);
            }
            return strBldr.ToString();
        }
    }

}
