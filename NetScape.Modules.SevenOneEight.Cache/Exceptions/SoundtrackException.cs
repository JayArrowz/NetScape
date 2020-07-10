using System;

namespace NetScape.Modules.SevenOneEight.Cache.Exceptions
{
    [Serializable]
    public class SoundtrackException : Exception
    {
        public SoundtrackException()
        {
        }

        public SoundtrackException(string message) : base(message)
        {
        }

        public SoundtrackException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}