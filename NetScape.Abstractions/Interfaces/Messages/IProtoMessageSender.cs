using Google.Protobuf;
using NetScape.Abstractions.Model.Game;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.Messages
{
    public interface IProtoMessageSender
    {
        Task SendAsync(Player player, IMessage message);
    }
}