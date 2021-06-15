using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Interfaces.World;
using NetScape.Modules.FiveZeroEight.LoginProtocol.WorldList;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers
{
    public class WorldListDecoder : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly IWorld _world;
        private static Country[] Countries { get; } = { new Country(Country.FLAG_UK, "UK") };
        public WorldListDecoder(IWorld world)
        {
            _world = world;
        }
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            if (msg.ReadableBytes < 4)
            {
                return;
            }

            int sessionId = msg.ReadInt();
            World[] worlds = { new World(1, World.FLAG_MEMBERS | World.FLAG_HIGHLIGHT, 0, "-", "127.0.0.1") };
            int[] players = { _world.Players.Count };
            _ = ctx.Channel.WriteAndFlushAsync(new WorldListMessage(sessionId, Countries, worlds, players))
                .ContinueWith(_ => _ = ctx.CloseAsync());
        }
    }
}
