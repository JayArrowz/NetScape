using NetScape.Abstractions.Util;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public interface ICipheredHandler
    {
        IsaacRandom Cipher { get; set; }
    }
}
