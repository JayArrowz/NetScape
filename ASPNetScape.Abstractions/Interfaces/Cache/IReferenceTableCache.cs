using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ASPNetScape.Abstractions.Cache;

namespace ASPNetScape.Abstractions.Interfaces.Cache
{
    public interface IReferenceTableCache : IDisposable
    {
        ConcurrentDictionary<CacheIndex, ReferenceTableFile> CachedReferenceTables { get; set; }
        void FlushCachedReferenceTables();
        IEnumerable<int> GetFileIds(CacheIndex index);
        CacheFileInfo GetFileInfo(CacheIndex index, int fileId);
        ReferenceTableFile GetReferenceTable(CacheIndex index, bool createIfNotFound = false);
        T GetFile<T>(CacheIndex index, int fileId) where T : CacheFileBase;
        IEnumerable<CacheIndex> GetIndexes();
    }
}