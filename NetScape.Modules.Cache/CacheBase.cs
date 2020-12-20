using System;
using System.Collections.Generic;
using NetScape.Abstractions.Cache;
using NetScape.Modules.Cache.FileTypes;

namespace NetScape.Modules.Cache
{
    /// <summary>
    /// Base class for cache systems.
    /// </summary>
    public abstract class CacheBase : IDisposable
    {
        /// <summary>
        /// Returns the indexes available in the cache.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<CacheIndex> GetIndexes();

        /// <summary>
        /// Returns the files available in the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract IEnumerable<int> GetFileIds(CacheIndex index);

        /// <summary>
        /// Returns info on the specified file without actually obtaining the file.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public abstract CacheFileInfo GetFileInfo(CacheIndex index, int fileId);

        /// <summary>
        /// Writes the given info to the cache.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        protected abstract void PutFileInfo(CacheFileInfo fileInfo);

        /// <summary>
        /// Returns the requested file converted to the requested type.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public T GetFile<T>(CacheIndex index, int fileId) where T : CacheFileBase
        {
            // Obtain the file
            var info = this.GetFileInfo(index, fileId);
            var file = this.GetBinaryFile(info);

            // Return the file as is when a binary file is requested
            if (typeof(T) == typeof(BinaryFile))
            {
                return file as T;
            }

            // Decode the file to the requested type
            var decodedFile = Activator.CreateInstance<T>();
            decodedFile.FromBinaryFile(file);
            return decodedFile;
        }

        /// <summary>
        /// Implements the logic for actually retrieving file from the cache.
        /// </summary>
        /// <returns></returns>
        protected abstract BinaryFile GetBinaryFile(CacheFileInfo fileInfo);

        /// <summary>
        /// Writes a file to the cache.
        /// The file's info will be used to determine where and how to put the file in the cache.
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="ArgumentException"></exception>
        public void PutFile(CacheFileBase file)
        {
            if (file.Info.Index == CacheIndex.Undefined || file.Info.FileId == null)
            {
                throw new ArgumentException("A file must have an index and file id to be written.");
            }

            // TODO: #57
            if (file.Info.EntryId != null)
            {
                throw new ArgumentException("Entries can not be directly written to the cache. Use an entry file containing entries or remove the entry id from its info.");
            }

            var binaryFile = file.ToBinaryFile();

            this.PutBinaryFile(binaryFile);
            this.PutFileInfo(binaryFile.Info);
        }

        protected abstract void PutBinaryFile(BinaryFile file);

        /// <summary>
        /// Copies the specified file over to the given cache.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fileId"></param>
        /// <param name="cache"></param>
        public void CopyFile(CacheIndex index, int fileId, CacheBase cache)
        {
            cache.PutBinaryFile(this.GetFile<BinaryFile>(index, fileId));
        }

        public virtual void Dispose()
        {
        }
    }
}
