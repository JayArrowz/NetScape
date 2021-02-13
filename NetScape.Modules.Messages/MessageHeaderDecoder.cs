using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
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

        public static readonly int[] PacketLengths = {
            0, 0, 0, 1, -1, 0, 0, 0, 0, 0, // 0
			0, 0, 0, 0, 8, 0, 6, 2, 2, 0, // 10
			0, 2, 0, 6, 0, 12, 0, 0, 0, 0, // 20
			0, 0, 0, 0, 0, 8, 4, 0, 0, 2, // 30
			2, 6, 0, 6, 0, -1, 0, 0, 0, 0, // 40
			0, 0, 0, 12, 0, 0, 0, 8, 8, 0, // 50
			0, 8, 0, 0, 0, 0, 0, 0, 0, 0, // 60
			6, 0, 2, 2, 8, 6, 0, -1, 0, 6, // 70
			0, 0, 0, 0, 0, 1, 4, 6, 0, 0, // 80
			0, 0, 0, 0, 0, 3, 0, 0, -1, 0, // 90
			0, 13, 0, -1, 0, 0, 0, 0, 0, 0, // 100
			0, 0, 0, 0, 0, 0, 0, 6, 0, 0, // 110
			1, 0, 6, 0, 0, 0, -1, 0, 2, 6, // 120
			0, 4, 6, 8, 0, 6, 0, 0, 0, 2, // 130
			0, 0, 0, 0, 0, 6, 0, 0, 0, 0, // 140
			0, 0, 1, 2, 0, 2, 6, 0, 0, 0, // 150
			0, 0, 0, 0, -1, -1, 0, 0, 0, 0, // 160
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 170
			0, 8, 0, 3, 0, 2, 0, 0, 8, 1, // 180
			0, 0, 12, 0, 0, 0, 0, 0, 0, 0, // 190
			2, 0, 0, 0, 0, 0, 0, 0, 4, 0, // 200
			4, 0, 0, 0, 7, 8, 0, 0, 10, 0, // 210
			0, 0, 0, 0, 0, 0, -1, 0, 6, 0, // 220
			1, 0, 0, 0, 6, 0, 6, 8, 1, 0, // 230
			0, 4, 0, 0, 0, 0, -1, 0, -1, 4, // 240
			0, 0, 6, 6, 0, 0, // 250
        };

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var isaacValue = CipherPair.DecodingRandom.NextInt();
            int opcode = input.ReadByte();
            int unencodedOpcode = opcode - isaacValue & 0xFF;
            var decoder = _messageDecoders.FirstOrDefault(decoder => decoder.Ids.Contains(unencodedOpcode));
            var frameType = decoder?.FrameType ?? FrameType.Fixed;
            var size = 0;

            if (frameType != FrameType.Fixed)
            {
                size = frameType.GetBytes(input);
            }
            else
            {
                size = PacketLengths[unencodedOpcode];
            }

            var buffer = input.ReadBytes(size);
            if (decoder == null)
            {
                Log.Logger.Warning("Opcode {0} not recognised", unencodedOpcode);
                return;
            }
            Log.Logger.Debug("Decoding Opcode: {0} Player Name: {1} from {2} Size {3}", unencodedOpcode, Player.Username, context.Channel.RemoteAddress, size);
            decoder.DecodeAndPublish(Player, new MessageFrame(unencodedOpcode, decoder.FrameType, buffer));
        }
    }
}
