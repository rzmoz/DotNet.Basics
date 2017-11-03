using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class ServiceProviderNullReferenceException : Exception
    {
        public ServiceProviderNullReferenceException()
        {
        }

        protected ServiceProviderNullReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceProviderNullReferenceException(string message) : base(message)
        {
        }

        public ServiceProviderNullReferenceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
