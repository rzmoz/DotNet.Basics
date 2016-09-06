using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
