using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Cache;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.Interfaces.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.Osrs.LoginProtocol
{
    public class JS5Encoder : MessageToByteEncoder<JS5Request>
    {
        private readonly IReferenceTableCache _referenceTableCache;
        private readonly IFileStore _fileStore;

        public JS5Encoder(IReferenceTableCache referenceTableCache, IFileStore fileStore)
        {
            _referenceTableCache = referenceTableCache;
            _fileStore = fileStore;
        }

        protected override void Encode(IChannelHandlerContext context, JS5Request message, IByteBuffer output)
        {
            try
            {
                int index = message.Index;
                int file = message.File;

                output.WriteByte(index);
                output.WriteShort(file);

                byte[] fileData;
                if (index == 255 && file == 255)
                    fileData = GetIndexSizeData(context);
                else if (index == 255)
                    fileData = GetReferenceTableData(file);
                else
                    fileData = GetFileData(index, file);

                foreach (byte b in fileData)
                {
                    if (output.WriterIndex % 512 == 0)
                    {
                        output.WriteByte(-1);
                    }

                    output.WriteByte(b);
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Logger.Error(e, nameof(Encode));
            }
        }
        private byte[] GetReferenceTableData(int file)
        {

            return _referenceTableCache.GetFile<BinaryFile>(CacheIndex.ReferenceTables, file).Encode();
        }

        private byte[] GetFileData(int index, int file)
        {
            var cacheIndex = (CacheIndex)index;
            return _referenceTableCache.GetFile<BinaryFile>(cacheIndex, file).Data;
        }

        private byte[] Trim(byte[] b)
        {
            var buffer = Unpooled.WrappedBuffer(b);
            int compression = buffer.ReadByte();
            int size = buffer.ReadInt();

            byte[] n = new byte[size + (compression == 0 ? 5 : 9)];
            Array.Copy(b, 0, n, 0, size + (compression == 0 ? 5 : 9));


            /*var compressionExtraSize = _referenceTableCache.CachedReferenceTables[(CacheIndex)file].Info.CompressionType == CompressionType.None ? 5 : 9;
            var compressedSize = _referenceTableCache.CachedReferenceTables[(CacheIndex)file].Info.CompressedSize.GetValueOrDefault();

            byte[] n = new byte[compressionExtraSize + compressedSize];
            Array.Copy(b, 0, n, 0, compressionExtraSize + compressedSize);*/
            return n;
        }

        private byte[] GetIndexSizeData(IChannelHandlerContext ctx)
        {

            var indexes = _referenceTableCache.GetIndexes().ToList();
            var indexSize = indexes.Count;

            var buffer = ctx.Allocator.Buffer(5 + (indexSize * 8));

            try
            {
                buffer.WriteByte(0);
                buffer.WriteInt(indexSize * 8);
                for (int index = 0; index < indexSize; index++)
                {
                    var file = _referenceTableCache.GetReferenceTable(indexes[index]).Info;
                    buffer.WriteInt(file.Crc.Value);
                    buffer.WriteInt(file.Version.GetValueOrDefault());
                    Serilog.Log.Logger.Debug("Sending Index {0} File CRC: {1} Version: {2}", index, file.Crc, file.Version);
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Logger.Error(e, nameof(GetIndexSizeData));
            }
            return buffer.GetBytes();
        }
    }
}
