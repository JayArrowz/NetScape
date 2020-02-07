using ASPNetScape.Abstractions.Cache;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace ASPNetScape.Modules.SevenOneEight.Cache.Cache
{
    /// <summary>
    /// A cache that stores information on its files in reference tables in index 255.
    /// </summary>
    public abstract class ReferenceTableCacheBase : CacheBase
    {
        public ConcurrentDictionary<CacheIndex, ReferenceTableFile> CachedReferenceTables { get; set; } =
            new ConcurrentDictionary<CacheIndex, ReferenceTableFile>();

        private List<CacheIndex> _changedReferenceTableIndexes = new List<CacheIndex>();

        public ReferenceTableFile GetReferenceTable(CacheIndex index, bool createIfNotFound = false)
        {
            // Obtain the reference table either from our own cache or the actual cache
            return CachedReferenceTables.GetOrAdd(index, regardlesslyDiscarded =>
            {
                try
                {
                    return this.GetFile<ReferenceTableFile>(CacheIndex.ReferenceTables, (int)index);
                }
                catch (FileNotFoundException) when (createIfNotFound)
                {
                    return new ReferenceTableFile
                    {
                        Info = new CacheFileInfo
                        {
                            Index = CacheIndex.ReferenceTables,
                            FileId = (int)index
                        }
                    };
                }
            });
        }

        public sealed override CacheFileInfo GetFileInfo(CacheIndex index, int fileId)
        {
            if (index != CacheIndex.ReferenceTables)
            {
                return this.GetReferenceTable(index).GetFileInfo(fileId);
            }

            return new CacheFileInfo
            {
                Index = index,
                FileId = fileId
                // TODO: Compression for reference tables? Compression by default?
            };
        }

        protected sealed override void PutFileInfo(CacheFileInfo fileInfo)
        {
            // Reference tables don't need no reference tables of their own 
            if (fileInfo.Index != CacheIndex.ReferenceTables)
            {
                this.GetReferenceTable(fileInfo.Index, true).SetFileInfo(fileInfo.FileId.Value, fileInfo);
                this._changedReferenceTableIndexes.Add(fileInfo.Index);
            }
        }

        public sealed override IEnumerable<int> GetFileIds(CacheIndex index)
        {
            return this.GetReferenceTable(index).FileIds;
        }

        /// <summary>
        /// Writes changes made to the locally cached reference tables and clears the local cache.
        /// </summary>
        public void FlushCachedReferenceTables()
        {
            foreach (var tableIndex in this._changedReferenceTableIndexes)
            {
                this.PutFile(CachedReferenceTables[tableIndex]);
            }

            this._changedReferenceTableIndexes.Clear();
            CachedReferenceTables.Clear();
        }

        public override void Dispose()
        {
            this.FlushCachedReferenceTables();

            base.Dispose();

            CachedReferenceTables = null;
            this._changedReferenceTableIndexes = null;
        }
    }
}