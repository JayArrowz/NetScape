using NetScape.Modules.Messages.Builder;

namespace NetScape.Messages.Generator
{
    public class MessageParameters
    {
        public string Name { get; set; }
        public MessageType Type { get; set; }
        public DataOrder? Order { get; set; }
        public DataTransformation? Transform { get; set; }
    }
}