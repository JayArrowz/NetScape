using ASPNetScape.Abstractions.FileSystem;
using System;
using System.IO;

namespace ASPNetScape
{
    public class FileSystem : IFileSystem
    {
        private const string _baseFolder = "AspNetServerData/";
        private const string _cacheFolder = "cache/";

        public string BasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), _baseFolder);
        public string CachePath => Path.Combine(BasePath, _cacheFolder);
    }
}
