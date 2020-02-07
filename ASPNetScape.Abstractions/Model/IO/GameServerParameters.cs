using ASPNetScape.Abstractions.Interfaces.IO;

namespace ASPNetScape.Abstractions.Model.IO
{
    public sealed class GameServerParameters : IGameServerParameters
    {
        public string BindAddress { get; set; }
        public ushort Port { get; set; }
    }
}
