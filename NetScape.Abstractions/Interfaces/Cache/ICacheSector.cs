using NetScape.Abstractions.Cache;

namespace NetScape.Abstractions.Interfaces.Cache
{
    public interface ICacheSector
    {
        int ChunkIndex { get; }
        bool Extended { get; }
        int FileId { get; }
        CacheIndex Index { get; }
        int? NextSectorPosition { get; set; }
        byte[] Payload { get; }
        int Position { get; set; }
        byte[] Encode();
    }
}