using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetScape.Abstractions.Cache;
using NetScape.Abstractions.Extensions;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Cache;
using Autofac;
using NetScape.Modules.Cache.FileTypes;

namespace NetScape.Modules.Cache.RuneTek5
{
    /// <summary>
    ///     A file store holds multiple files inside a "virtual" file system made up of several index files and a single data
    ///     file.
    /// </summary>
    /// <author>Graham</author>
    /// <author>`Discardedx2</author>
    /// <author>Villermen</author>
    // TODO: See if this class can be replaced by RuneTek5Cache and a Sector class replacement that can read full files from a stream.
    public class FileStore : IFileStore, IStartable
    {
        /// <summary>
        ///     Lock that is used when reading data from the streams.
        /// </summary>
        private readonly object _ioLock = new object();

        private const int IndexPointerSize = 6; // filesize + firstSectorPosition

        private readonly Dictionary<CacheIndex, Stream> _indexStreams = new Dictionary<CacheIndex, Stream>();
        private Dictionary<CacheIndex, Dictionary<int, byte[]>> CachedStorage { get; } = new();

        private Stream _dataStream;

        /// <summary>
        ///     Opens the file store in the specified directory.
        /// </summary>
        public FileStore(IFileSystem fileSystem)
        {
            this.CacheDirectory = PathExtensions.FixDirectory(fileSystem.CachePath);
            this.ReadOnly = true;
        }

        public bool ReadOnly { get; private set; }
        public string CacheDirectory { get; private set; }

        /// <summary>
        ///     The loaded/existing indexes.
        /// </summary>
        public IEnumerable<CacheIndex> GetIndexes()
        {
            return this._indexStreams.Keys.Where(index => index != CacheIndex.ReferenceTables);
        }

        private IEnumerable<Sector> ReadSectors(CacheIndex index, int fileId)
        {
            int filesize;
            return this.ReadSectors(index, fileId, out filesize);
        }

        /// <summary>
        /// Reads the sectors
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public byte[] ReadFileData(CacheIndex index, int fileId)
        {
            if(CachedStorage.ContainsKey(index))
            {
                if(CachedStorage[index].ContainsKey(fileId))
                {
                    return CachedStorage[index][fileId];
                }
            } else
            {
                CachedStorage.Add(index, new());
            }

            int filesize;
            var fileData = this.ReadSectors(index, fileId, out filesize).Aggregate(new List<byte>(), (bytes, sector) =>
            {
                bytes.AddRange(sector.Payload);
                return bytes;
            }).Take(filesize).ToArray();
            CachedStorage[index].Add(fileId, fileData);
            return fileData;

        }

        private IEnumerable<Sector> ReadSectors(CacheIndex index, int fileId, out int filesize)
        {
            if (!this._indexStreams.ContainsKey(index))
            {
                throw new FileNotFoundException($"Cannot read from index {(int)index} as it does not exist.");
            }

            lock (this._ioLock)
            {
                var indexReader = new BinaryReader(this._indexStreams[index]);
                var indexPosition = (long)fileId * IndexPointerSize;
                if (indexPosition < 0 || indexPosition >= indexReader.BaseStream.Length)
                {
                    throw new FileNotFoundException($"File {fileId} is outside of index {(int)index}'s file bounds.");
                }

                var sectors = new List<Sector>();
                indexReader.BaseStream.Position = indexPosition;

                filesize = indexReader.ReadUInt24BigEndian();
                var firstSectorPosition = indexReader.ReadUInt24BigEndian();
                if (filesize <= 0)
                {
                    throw new FileNotFoundException(
                        $"File {fileId} in index {(int)index} has no size meaning it is not stored in the cache."
                    );
                }

                var chunkId = 0;
                var remaining = filesize;
                var dataReader = new BinaryReader(this._dataStream);
                var sectorPosition = firstSectorPosition;
                do
                {
                    dataReader.BaseStream.Position = sectorPosition * Sector.Size;

                    var sectorBytes = dataReader.ReadBytesExactly(Sector.Size);
                    var sector = Sector.Decode(sectorPosition, sectorBytes, index, fileId, chunkId++);

                    var bytesRead = Math.Min(sector.Payload.Length, remaining);

                    remaining -= bytesRead;

                    sectors.Add(sector);

                    sectorPosition = sector.NextSectorPosition.Value;
                }
                while (remaining > 0);

                return sectors;

            }
        }
        /// <summary>
        /// If available, overwrites the space allocated to the previous file first to save space.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fileId"></param>
        /// <param name="data"></param>
        public void WriteFileData(CacheIndex index, int fileId, byte[] data)
        {
            if (this.ReadOnly)
            {
                throw new InvalidOperationException("Can't write data in readonly mode.");
            }

            lock (this._ioLock)
            {
                // Obtain possibly existing sector positions to overwrite
                int[] existingSectorPositions;
                try
                {
                    existingSectorPositions = this.ReadSectors(index, fileId)
                        .Select(sector => sector.Position)
                        .ToArray();
                }
                catch (Exception ex) when (ex is FileNotFoundException)
                {
                    // Assume there are no existing sectors when the method fails
                    existingSectorPositions = new int[0];
                }

                var sectors = Sector.FromData(data, index, fileId);

                var dataWriter = new BinaryWriter(this._dataStream);

                foreach (var sector in sectors)
                {
                    // Overwrite existing sector data if available, otherwise append to file
                    sector.Position = sector.ChunkIndex < existingSectorPositions.Length
                        ? existingSectorPositions[sector.ChunkIndex]
                        : (int)(dataWriter.BaseStream.Length / Sector.Size);

                    // Set position of next sector
                    sector.NextSectorPosition = sector.ChunkIndex + 1 < existingSectorPositions.Length
                        ? existingSectorPositions[sector.ChunkIndex + 1]
                        : (int)(dataWriter.BaseStream.Length / Sector.Size);

                    // Happens if both positions were based on the stream length
                    if (sector.NextSectorPosition == sector.Position)
                    {
                        sector.NextSectorPosition++;
                    }

                    // Add to index
                    if (sector.ChunkIndex == 0)
                    {
                        var pointer = new IndexPointer
                        {
                            FirstSectorPosition = sector.Position,
                            Filesize = data.Length
                        };

                        // Create index file if it does not exist yet
                        if (!this._indexStreams.ContainsKey(index))
                        {
                            this._indexStreams.Add(index, File.Open(
                                Path.Combine(this.CacheDirectory, "main_file_cache.idx" + (int)index),
                                FileMode.OpenOrCreate,
                                FileAccess.ReadWrite));
                        }

                        var indexWriter = new BinaryWriter(this._indexStreams[index]);
                        var pointerPosition = fileId * IndexPointer.Length;

                        // Write zeroes up to the desired position of the index stream if it is larger than its size
                        if (indexWriter.BaseStream.Length < pointerPosition)
                        {
                            indexWriter.BaseStream.Position = indexWriter.BaseStream.Length;
                            indexWriter.Write(Enumerable.Repeat((byte)0, (int)(pointerPosition - indexWriter.BaseStream.Length)).ToArray());
                        }
                        else
                        {
                            indexWriter.BaseStream.Position = pointerPosition;
                        }

                        pointer.Encode(indexWriter.BaseStream);
                    }

                    // Write the encoded sector
                    dataWriter.BaseStream.Position = sector.Position * Sector.Size;
                    dataWriter.Write(sector.Encode());
                }
            }
        }

        public void Dispose()
        {
            this._dataStream.Dispose();

            foreach (var indexStream in this._indexStreams.Values)
            {
                indexStream.Dispose();
            }
        }

        public void Start()
        {
            if (!this.ReadOnly)
            {
                Directory.CreateDirectory(this.CacheDirectory);
            }

            var fileAccess = this.ReadOnly ? FileAccess.Read : FileAccess.ReadWrite;

            var dataFilePath = Path.Combine(this.CacheDirectory, "main_file_cache.dat");

            if (this.ReadOnly && !File.Exists(dataFilePath))
            {
                throw new FileNotFoundException("Cache data file does not exist.");
            }

            this._dataStream = File.Open(dataFilePath, FileMode.OpenOrCreate, fileAccess);

            // Load in existing index files
            for (var indexId = 0; indexId <= 255; indexId++)
            {
                var indexFile = Path.Combine(this.CacheDirectory, "main_file_cache.idx" + indexId);

                if (!File.Exists(indexFile))
                {
                    continue;
                }

                this._indexStreams.Add((CacheIndex)indexId, File.Open(indexFile, FileMode.Open, fileAccess));
            }
        }
    }
}