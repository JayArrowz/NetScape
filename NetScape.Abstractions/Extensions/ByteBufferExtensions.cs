using DotNetty.Buffers;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NetScape.Abstractions.Extensions
{
    public static class ByteBufferExtensions
    {
        public static string ReadString(this IByteBuffer buffer)
        {
            return ReadString(buffer, 10);
        }

        public static string ReadString(this IByteBuffer buffer, int terminator)
        {
            var strBldr = new StringBuilder();
            int charByte;
            while ((charByte = buffer.ReadByte()) != terminator)
            {
                strBldr.Append((char)charByte);
            }
            return strBldr.ToString();
        }

        public static IByteBuffer CompressGzip(this IByteBuffer buffer)
        {
            var data = buffer.GetBytes();
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return Unpooled.WrappedBuffer(compressedStream.ToArray());
            }
        }

        public static byte[] GetBytes(this IByteBuffer buffer)
        {
            buffer.SetReaderIndex(0);
            byte[] copiedBuffer = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(copiedBuffer);
            return copiedBuffer;
        }

        public static IByteBuffer DecompressGzip(this IByteBuffer buffer)
        {
            var data = buffer.GetBytes();
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return Unpooled.WrappedBuffer(resultStream.ToArray());
            }
        }

        public static void DecodeXtea(this IByteBuffer buffer, uint[] keys)
        {
            DecodeXtea(buffer, keys, 0, keys.Length);
        }

        public static void DecodeXtea(this IByteBuffer buffer, uint[] keys, int start, int end)
        {
            int l = buffer.ReaderIndex;
            buffer.SetReaderIndex(start);
            int i1 = (end - start) / 8;
            for (int j1 = 0; j1 < i1; j1++)
            {
                uint k1 = buffer.ReadUnsignedInt();
                uint l1 = buffer.ReadUnsignedInt();
                uint sum = 0xc6ef3720;
                uint delta = 0x9e3779b9;
                for (int k2 = 32; k2-- > 0;)
                {
                    l1 -= keys[(sum & 0x1c84) >> 11] + sum ^ (k1 >> 5 ^ k1 << 4)
                            + k1;
                    sum -= delta;
                    k1 -= (l1 >> 5 ^ l1 << 4) + l1 ^ keys[sum & 3] + sum;
                }
                buffer.SetReaderIndex(buffer.ReaderIndex - 8);
                buffer.WriteInt((int) k1);
                buffer.WriteInt((int) l1);
            }
            buffer.SetReaderIndex(l);
        }

        public static void EncodeXtea(this IByteBuffer buffer, uint[] keys, int start, int end)
        {
            int o = buffer.WriterIndex;
            int j = (end - start) / 8;
            buffer.SetWriterIndex(start);
            for (int k = 0; k < j; k++)
            {
                uint l = buffer.ReadUnsignedInt();
                uint i1 = buffer.ReadUnsignedInt();
                uint sum = 0;
                uint delta = 0x9e3779b9;
                for (int l1 = 32; l1-- > 0;)
                {
                    l += sum + keys[3 & sum] ^ i1 + (i1 >> 5 ^ i1 << 4);
                    sum += delta;
                    i1 += l + (l >> 5 ^ l << 4) ^ keys[(0x1eec & sum) >> 11]
                            + sum;
                }

                buffer.SetWriterIndex(buffer.WriterIndex - 8);
                buffer.WriteInt((int) l);
                buffer.WriteInt((int) i1);
            }
            buffer.SetWriterIndex(o);
        }
    }
}
