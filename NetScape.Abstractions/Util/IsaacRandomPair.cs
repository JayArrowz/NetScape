using NetScape.Abstractions.Util;

namespace NetScape.Abstractions.IO.Util
{
    public class IsaacRandomPair
    {
        public IsaacRandom EncodingRandom { get; }
        public IsaacRandom DecodingRandom { get; }

        public IsaacRandomPair(IsaacRandom encodingRandom, IsaacRandom decodingRandom)
        {
            this.EncodingRandom = encodingRandom;
            this.DecodingRandom = decodingRandom;
        }
    }
}