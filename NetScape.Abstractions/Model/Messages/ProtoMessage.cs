using Google.Protobuf;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Model.Messages
{
    public class ProtoMessage
    {
        public Player Player { get; }
        public int Opcode { get; }
        public IMessage Message { get; }

        public ProtoMessage(int opcode, Player player, IMessage message)
        {
            Player = player;
            Opcode = opcode;
            Message = message;
        }
    }
}
