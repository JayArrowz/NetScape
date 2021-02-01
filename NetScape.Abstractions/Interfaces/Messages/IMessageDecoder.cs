using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public interface IMessageDecoder
    {
        int[] Ids { get; }
        FrameType FrameType { get; }
        void Decode(Player player, MessageFrame frame);
    }
}
