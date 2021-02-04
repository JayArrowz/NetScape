using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Messages;

namespace NetScape.Modules.Messages.Decoders.Messages
{
    public class WalkingQueueMessage : DecoderMessage
    {
        public bool Run { get; set; }
        public Position[] Positions { get; set; }
    }
}
