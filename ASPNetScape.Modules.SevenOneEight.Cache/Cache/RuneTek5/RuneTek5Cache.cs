using ASPNetScape.Abstractions.Cache;
using ASPNetScape.Abstractions.Interfaces.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASPNetScape.Modules.SevenOneEight.Cache.Cache.RuneTek5
{
    /// <summary>
    /// Can read and write to a RuneTek5 type cache consisting of a single data (.dat2) file and some index (.id#) files.
    /// </summary>
    /// <author>Graham</author>
    /// <author>`Discardedx2</author>
    /// <author>Villermen</author>
    public class RuneTek5Cache : ReferenceTableCacheBase, IReferenceTableCache
    {
        /// <summary>
        /// The <see cref="RuneTek5.FileStore" /> that backs this cache.
        /// </summary>
        private IFileStore _fileStore;

        /// <summary>
        /// Creates an interface on the cache stored in the given directory.
        /// </summary>
        /// <param name="cacheDirectory"></param>
        /// <param name="readOnly"></param>
        public RuneTek5Cache(IFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        public override IEnumerable<CacheIndex> GetIndexes()
        {
            return this._fileStore.GetIndexes();
        }

        protected override BinaryFile GetBinaryFile(CacheFileInfo fileInfo)
        {
            var file = new BinaryFile
            {
                Info = fileInfo
            };

            file.Decode(this._fileStore.ReadFileData(fileInfo.Index, fileInfo.FileId.Value));

            return file;
        }

        protected override void PutBinaryFile(BinaryFile file)
        {
            // Write data to file store
            this._fileStore.WriteFileData(file.Info.Index, file.Info.FileId.Value, file.Encode());
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this._fileStore != null)
            {
                this._fileStore.Dispose();
                this._fileStore = null;
            }
        }
    }
}
