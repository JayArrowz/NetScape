using System;

namespace NetScape.Modules.Messages.Generator
{
    [Obsolete]
    public class FrameParameters
    {
        public string Namespace { get; set; }
        public GeneratorMessageType Type { get; set; }
        public GeneratorData[] Messages { get; set; }
    }

    [Obsolete]
    public enum GeneratorMessageType
    {
        Decoder,
        Encoder
    }
}
