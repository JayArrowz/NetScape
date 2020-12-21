using NetScape.Modules.Messages.Builder;

namespace NetScape.Messages.Generator
{
    public class GeneratorData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public MessageParams[] Params { get; set; }
        public MessageFrame.MessageType FrameType { get; set; }
    }

    public class MessageParams
    {
        public string Name { get; set; }
        public MessageType Type { get; set; }
        public DataOrder Order { get; set; }
        public DataTransformation Transform { get; set; }
    }
}