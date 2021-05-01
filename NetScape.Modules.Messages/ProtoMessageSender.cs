using Google.Protobuf;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Messages;
using System.Threading.Tasks;

namespace NetScape.Modules.Messages
{
    public class ProtoMessageSender : IProtoMessageSender
    {
        private readonly ProtoMessageCodecHandler _protoMessageCodecHandler;
        public ProtoMessageSender(ProtoMessageCodecHandler protoMessageCodecHandler)
        {
            _protoMessageCodecHandler = protoMessageCodecHandler;
        }

        public Task SendAsync(Player player, IMessage message)
        {
            var opcode = _protoMessageCodecHandler.EncoderTypeMap[message.Descriptor.ClrType];
            return player.ChannelHandlerContext.Channel.WriteAndFlushAsync(new ProtoMessage(opcode, player, message));
        }
    }
}
