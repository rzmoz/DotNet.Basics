using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class ServiceProviderIsNullException : Exception
    {
        public ServiceProviderIsNullException()
        {
        }

        protected ServiceProviderIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceProviderIsNullException(string message) : base(message)
        {
        }

        public ServiceProviderIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
