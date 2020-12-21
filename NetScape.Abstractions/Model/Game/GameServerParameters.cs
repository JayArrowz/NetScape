using NetScape.Abstractions.Interfaces.IO;

namespace NetScape.Abstractions.Model.Game
{
    public sealed class GameServerParameters : IGameServerParameters
    {
        public string BindAddress { get; set; }
        public ushort Port { get; set; }
    }
}
