using ASPNetScape.Abstractions.Cache;
using System;
using System.Collections.Generic;

namespace ASPNetScape.Abstractions.Interfaces.Cache
{
    public interface IFileStore : IDisposable
    {
        string CacheDirectory { get; }
        bool ReadOnly { get; }
        IEnumerable<CacheIndex> GetIndexes();
        byte[] ReadFileData(CacheIndex index, int value);
        void WriteFileData(CacheIndex index, int value, byte[] v);
    }
}