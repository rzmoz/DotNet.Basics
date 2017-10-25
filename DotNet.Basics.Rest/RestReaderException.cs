using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Rest
{
    public class RestReaderException : Exception
    {
        public RestReaderException(string message) : base(message)
        {
        }

        public RestReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RestReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
