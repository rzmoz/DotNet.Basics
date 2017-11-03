using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class LazyLoadTaskResolvedToNullException : Exception
    {
        public LazyLoadTaskResolvedToNullException()
        {
        }

        protected LazyLoadTaskResolvedToNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public LazyLoadTaskResolvedToNullException(string message) : base(message)
        {
        }

        public LazyLoadTaskResolvedToNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
