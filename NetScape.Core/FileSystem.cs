using NetScape.Abstractions.FileSystem;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace NetScape.Core
{
    public class FileSystem : IFileSystem
    {
        private readonly FileSystemConfig _fileConfig;

        public FileSystem(IConfigurationRoot configurationRoot)
        {
            _fileConfig = configurationRoot.GetSection("FileSystem").Get<FileSystemConfig>();
            BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                _fileConfig.BaseFolder);
        }

        public string BasePath { get; }

        public string CachePath => Path.Combine(BasePath, _fileConfig.CacheFolder);
    }

    public class FileSystemConfig
    {
        public string BaseFolder { get; set; }
        public string CacheFolder { get; set; }
    }
}
