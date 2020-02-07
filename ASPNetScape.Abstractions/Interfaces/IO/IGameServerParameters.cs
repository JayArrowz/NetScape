namespace ASPNetScape.Abstractions.Interfaces.IO
{
    public interface IGameServerParameters
    {
        string BindAddress { get; set; }

        ushort Port { get; set; }
    }
}
