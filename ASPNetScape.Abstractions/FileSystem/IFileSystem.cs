namespace ASPNetScape.Abstractions.FileSystem
{
    public interface IFileSystem
    {
        string BasePath { get; }
        string CachePath { get; }
    }
}
