using System;

namespace NetScape.Modules.SevenOneEight.Cache.Exceptions
{
    [Serializable]
    public class DownloaderException : Exception
    {
        public DownloaderException()
        {
        }

        public DownloaderException(string message) : base(message)
        {
        }

        public DownloaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}