using ASPNetScape.Abstractions;
using ASPNetScape.Abstractions.Cache;
using ASPNetScape.Abstractions.Extensions;
using ASPNetScape.Abstractions.Interfaces.Cache;
using ASPNetScape.Abstractions.Interfaces.IO;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Org.BouncyCastle.Crypto.Digests;
using Serilog;
using System;
using System.Linq;
using System.Numerics;
using Org.BouncyCastle.Utilities;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.JS5
{
    public class JS5PacketSender : IJS5PacketSender
    {
        private readonly IReferenceTableCache _referenceTableCache;
        private ILogger _logger;

        public JS5PacketSender(IReferenceTableCache referenceTableCache, ILogger logger)
        {
            _referenceTableCache = referenceTableCache;
            _logger = logger;
            LoadRefTables();
        }

        private void LoadRefTables()
        {
            var indexes = Enum.GetValues(typeof(CacheIndex)).Cast<CacheIndex>();
            foreach (var idx in indexes)
            {
                ReferenceTableFile refTable = null;

                try
                {
                    refTable = _referenceTableCache.GetReferenceTable(idx);
                }
                catch (Exception e)
                {
                    //_logger.Error($"Error decoding ref table {idx}: {e.ToString()}");
                }
            }
        }

        public IByteBuffer CreateUserKeysBuffer()
        {
            IByteBuffer stream = Unpooled.Buffer();

            var indexes = _referenceTableCache.CachedReferenceTables.Keys;
            var indexLength = indexes.Count;

            stream.WriteByte(indexLength);

            var orderedKeys = indexes.OrderBy(t => t);
            foreach (var key in orderedKeys)
            {
                ReferenceTableFile refTable = _referenceTableCache.CachedReferenceTables[key];
                var crc = refTable.Info.Crc.Value;
                var version = refTable.Version;
                var digest = refTable.Info.WhirlpoolDigest;
                stream.WriteInt(crc);
                stream.WriteInt(version);
                stream.WriteBytes(digest);

                //_logger.Verbose($"IDX: {(int)key} CRC: {crc} Version: {version}");
                //_logger.Verbose($"Whirpool: {string.Join(", ", digest)}");

            }

            byte[] archive = new byte[stream.WriterIndex];
            stream.SetReaderIndex(0);
            stream.ReadBytes(archive);
            //_logger.Verbose("ARCHIVE BYTES: " + string.Join(", ", archive));

            IByteBuffer hashStream = Unpooled.Buffer(65);
            hashStream.WriteByte(0);

            var whirlpool = new WhirlpoolDigest();
            whirlpool.BlockUpdate(archive, 0, archive.Length);
            var whirlpoolDigest = new byte[whirlpool.GetDigestSize()];
            whirlpool.DoFinal(whirlpoolDigest, 0);

            hashStream.WriteBytes(whirlpoolDigest);
            //_logger.Verbose("ARCHIVE HASH: " + string.Join(", ", whirlpoolDigest));

            var hash = Unpooled.WrappedBuffer(BigInteger
             .ModPow(new BigInteger(hashStream.GetBytes()), Constants.RSA_EXPONENT, Constants.RSA_MODULUS).ToByteArray());

            //TODO FIX
            sbyte[] hashPow =
            {
                45, 125, 123, 3, 36, -16, 0, 8, 86, -14, -121, 96, -31, 11, 31, -50, -62, -119, 16, -104, -87, 10, -74,
                -80, 5, 55, 99, -63, -48, -60, -86, -78, -51, -105, 67, -91, 6, -36, -61, 31, 74, -83, -21, -97, -105,
                -96, -4, 118, -64, -5, 48, 68, -64, -111, -85, 117, -11, 31, -110, -98, 42, 9, -101, 111, -120, 108, 88,
                101, 0, 112, 88, 92, -52, 49, -122, -67, -94, -18, 124, -5, 25, -2, 49, 113, -89, 80, 32, 112, -18, -32,
                6, -68, 42, -97, -58, -97, 116, 89, -118, -103, -49, -104, -2, 127, 53, 1, -110, 79, 53, -113, 73, -48,
                -3, -12, 2, 67, -34, -49, 12, 69, -62, 2, 116, 70, -75, 30, 45, -40
            };
            byte[] b = hashPow.Select(t => (byte)t).ToArray();
            stream.WriteBytes(b);
            stream.SetReaderIndex(0);
            return stream;
        }

        public void SendCacheArchive(IChannelHandlerContext ctx, int indexId, int archiveId, bool priority)
        {
            var channel = ctx.Channel;
            _logger.Verbose($"Channel: {channel.RemoteAddress.ToString()} Requesting Index: {indexId} Archive: {archiveId} Priority: {priority}");
            var refIdx = (int)CacheIndex.ReferenceTables;
            bool isRefTable = indexId == refIdx && archiveId == refIdx;

            if (isRefTable)
            {
                channel.WriteAndFlushAsync(GetContainerPacketData(255, 255, CreateUserKeysBuffer()));
                _logger.Verbose($"Sent UserKeys to Channel: {channel.RemoteAddress.ToString()}");
            }
            else
            {
                channel.WriteAndFlushAsync(GetArchivePacketData(indexId, archiveId, priority));
            }
        }

        public void SendStartup(IChannelHandlerContext ctx)
        {
            var buffer = Unpooled.Buffer(1 + Constants.Js5Keys.Length * 4);
            buffer.WriteByte(0);
            foreach (int key in Constants.Js5Keys)
                buffer.WriteInt(key);
            ctx.Channel.WriteAndFlushAsync(buffer);
        }

        /*
         * only using for ukeys atm, doesnt allow keys encode
         */
        public IByteBuffer GetContainerPacketData(int indexFileId, int containerId, IByteBuffer archiveBuffer)
        {
            var archive = archiveBuffer.GetBytes();
            var stream = Unpooled.Buffer(archive.Length + 4);
            stream.WriteByte(indexFileId);
            stream.WriteInt(containerId);
            stream.WriteByte(0);
            stream.WriteInt(archive.Length);
            int offset = 10;
            for (int index = 0; index < archive.Length; index++)
            {
                if (offset == 512)
                {
                    stream.WriteByte(255);
                    offset = 1;
                }
                stream.WriteByte(archive[index]);
                offset++;
            }

            //var bts = stream.GetBytes();
            //int[] a = bts.Select(t => t > sbyte.MaxValue ? t - 256 : t).ToArray();

            //_logger.Verbose("LEN: " + bts.Length + " SENDING UKEYS: " + string.Join(", ", bts));
            
            
            return stream;
        }

        private IByteBuffer GetArchivePacketData(int indexId, int archiveId, bool priority)
        {
            var file = _referenceTableCache.GetFile<BinaryFile>((CacheIndex)indexId, archiveId);

            var compType = file.Info.CompressionType;
            var compLegth = file.Info.CompressedSize.Value;

            int settings = (int)compType;
            if (!priority)
                settings |= 0x80;

            var buffer = Unpooled.Buffer();
            buffer.WriteByte(indexId);
            buffer.WriteInt(archiveId);
            buffer.WriteByte(settings);
            buffer.WriteInt(compLegth);

            int realLength = compType != CompressionType.None ? compLegth + 4 : compLegth;
            for (int index = 5; index < realLength + 5; index++)
            {
                if (buffer.WriterIndex % 512 == 0)
                {
                    buffer.WriteByte(255);
                }
                buffer.WriteByte(file.Data[index]);
            }

            return buffer;
        }
    }
}
