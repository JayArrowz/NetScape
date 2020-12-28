using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Model.Game;
using Serilog;
using System.Collections.Generic;
namespace NetScape.Modules.Messages
{
    public class MessageHeaderDecoder : ByteToMessageDecoder, ICipherAwareHandler, IPlayerAwareHandler
    {
        private readonly ILogger _logger;
        public MessageHeaderDecoder(ILogger logger)
        {
            _logger = logger;
        }

        public IsaacRandomPair CipherPair { get; set; }
        public Player Player { get; set; }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            int opcode = input.ReadByte();
            var isaacValue = CipherPair.DecodingRandom.NextInt();
            opcode = opcode - isaacValue & 0xFF;
            _logger.Debug("Decoding Opcode: {0} Player Name: {1} from {2}", opcode, Player.Username, context.Channel.RemoteAddress);
        }
    }
}
