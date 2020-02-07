using ASPNetScape.Abstractions.Cache;

namespace ASPNetScape.Abstractions.Interfaces.Cache
{
    public interface ICacheSector
    {
        int ChunkId { get; set; }
        byte[] Data { get; }
        int FileId { get; }
        CacheIndex Index { get; set; }
        bool IsExtended { get; }
        int NextSectorPosition { get; set; }
        int Position { get; set; }
        byte[] Encode();
    }
}