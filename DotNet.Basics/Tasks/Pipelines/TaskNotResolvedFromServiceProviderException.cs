using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class TaskNotResolvedFromServiceProviderException : Exception
    {
        public TaskNotResolvedFromServiceProviderException()
        { }

        protected TaskNotResolvedFromServiceProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public TaskNotResolvedFromServiceProviderException(string message) : base(message)
        { }

        public TaskNotResolvedFromServiceProviderException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
