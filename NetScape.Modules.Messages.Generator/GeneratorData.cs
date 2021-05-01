using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Messages.Generator
{
    [Obsolete]
    public class GeneratorData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public MessageParameters[] Params { get; set; }
        public FrameType FrameType { get; set; }
    }
}