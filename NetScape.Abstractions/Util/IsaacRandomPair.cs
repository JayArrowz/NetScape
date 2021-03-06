﻿namespace NetScape.Abstractions.Util
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