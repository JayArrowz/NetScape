using System;

namespace ASPNetScape.Abstractions.Cache
{
    [Serializable]
    public class DecodeException : Exception
    {
        public DecodeException()
        {
        }

        public DecodeException(string message) : base(message)
        {
        }

        public DecodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}