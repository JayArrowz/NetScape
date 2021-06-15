using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Extensions;
using NetScape.Modules.FiveZeroEight.LoginProtocol.WorldList;
using System.Linq;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers
{
    public class WorldListEncoder : MessageToByteEncoder<WorldListMessage>
    {
        protected override void Encode(IChannelHandlerContext context, WorldListMessage message, IByteBuffer output)
        {
            var buffer = context.Allocator.Buffer();
            buffer.WriteByte(1);
            buffer.WriteByte(1);

            var countries = message.Countries;
            buffer.WriteSmart(countries.Length);
            foreach (Country country in countries)
            {
                buffer.WriteSmart(country.Flag);
                buffer.WriteByte(0);
                buffer.Write8859String(country.Name);
            }

            int minWorldId = message.Worlds.Min(world => world.Id);
            int maxWorldId = message.Worlds.Max(world => world.Id);

            buffer.WriteSmart(minWorldId);
            buffer.WriteSmart(maxWorldId);
            buffer.WriteSmart(message.Worlds.Length);

            foreach (World world in message.Worlds)
            {
                buffer.WriteSmart(world.Id - minWorldId);
                buffer.WriteByte(world.Country);
                buffer.WriteInt(world.Flags);

                buffer.WriteByte(0);
                buffer.Write8859String(world.Activity);
                buffer.WriteByte(0);
                buffer.Write8859String(world.Ip);
            }

            buffer.WriteInt(message.SessionId);

            int[] players = message.Players;
            for (int i = 0; i < players.Length; i++)
            {
                var world = message.Worlds[i];
                buffer.WriteSmart(world.Id - minWorldId);
                buffer.WriteShort(players[i]);
            }

            output.WriteByte(0); // 0 = ok, 7/9 = world full
            output.WriteShort(buffer.ReadableBytes);
            output.WriteBytes(buffer);
        }
    }
}
