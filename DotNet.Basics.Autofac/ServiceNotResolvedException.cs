using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Autofac
{
    public class ServiceNotResolvedException : Exception
    {
        public ServiceNotResolvedException()
        {
        }

        protected ServiceNotResolvedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceNotResolvedException(string message) : base(message)
        {
        }

        public ServiceNotResolvedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
