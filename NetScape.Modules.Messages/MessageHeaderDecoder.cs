using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Model.Game;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace NetScape.Modules.Messages
{
    public class MessageHeaderDecoder : ByteToMessageDecoder, ICipherAwareHandler, IPlayerAwareHandler
    {
        private readonly IMessageDecoder[] _messageDecoders;
        public MessageHeaderDecoder(IMessageDecoder[] messageDecoders)
        {
            _messageDecoders = messageDecoders;
        }

        public IsaacRandomPair CipherPair { get; set; }
        public Player Player { get; set; }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var player = context.GetAttribute(Constants.PlayerAttributeKey).Get();
            int opcode = input.ReadByte();
            var isaacValue = CipherPair.DecodingRandom.NextInt();
            opcode = opcode - isaacValue & 0xFF;
            var decoder = _messageDecoders.FirstOrDefault(decoder => decoder.Ids.Contains(opcode));
            var frameType = decoder?.FrameType ?? Builder.FrameType.Fixed;
            var size = 0;

            if (frameType == Builder.FrameType.VariableByte)
            {
                size = input.ReadByte();
            }

            Log.Logger.Debug("Decoding Opcode: {0} Player Name: {1} from {2} Size {3}", opcode, Player.Username, context.Channel.RemoteAddress, size);

            if (decoder == null)
            {
                Log.Logger.Warning("Opcode {0} not recognised", opcode);
                return;
            }
            else
            {
                decoder.Decode(player, new Builder.MessageFrame(opcode, decoder.FrameType, input));
            }
        }
    }
}
