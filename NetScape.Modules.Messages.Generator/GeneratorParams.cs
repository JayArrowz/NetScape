using System.Collections.Generic;

namespace NetScape.Messages.Generator
{
    public class GeneratorParams
    {
        public string Namespace { get; set; }
        public GeneratorMessageType Type { get; set; }
        public GeneratorData[] Messages { get; set; }
    }

    public enum GeneratorMessageType
    {
        Decoder,
        Encoder
    }
}
