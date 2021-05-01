using Google.Protobuf;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Model.Messages
{
    public class DecoderMessage
    {
        public IMessage Message { get; set; }
        public Player Player { get; set; }
    }
}
