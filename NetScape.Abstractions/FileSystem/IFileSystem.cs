namespace NetScape.Abstractions.FileSystem
{
    public interface IFileSystem
    {
        /// <summary>
        /// Gets the base file system path.
        /// </summary>
        /// <value>
        /// The file system base path.
        /// </value>
        string BasePath { get; }

        /// <summary>
        /// Gets the cache path.
        /// </summary>
        /// <value>
        /// The cache path.
        /// </value>
        string CachePath { get; }
    }
}
