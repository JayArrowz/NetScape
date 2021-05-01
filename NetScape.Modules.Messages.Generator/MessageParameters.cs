using NetScape.Modules.Messages.Builder;
using System;

namespace NetScape.Messages.Generator
{
    [Obsolete]
    public class MessageParameters
    {
        public string Name { get; set; }
        public MessageType Type { get; set; }
        public DataOrder? Order { get; set; }
        public DataTransformation? Transform { get; set; }
        public bool Signed { get; set; }
    }
}