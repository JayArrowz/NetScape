using Google.Protobuf;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Model.Messages
{
    public class DecoderMessage<TMessage> where TMessage : IMessage<TMessage>
    {
        public TMessage Message { get; set; }
        public Player Player { get; set; }
    }
}
