using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Messages;

namespace NetScape.Modules.Messages.Decoders.Messages
{
    public class WalkingQueueMessage : DecoderMessage
    {
        public WalkingQueueMessage(bool run, Position[] positions)
        {
            Run = run;
            Positions = positions;
        }

        public bool Run { get; }
        public Position[] Positions { get; }
    }
}
