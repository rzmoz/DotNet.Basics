using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class TaskLoadFailedException : Exception
    {
        public TaskLoadFailedException()
        { }

        protected TaskLoadFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public TaskLoadFailedException(string message) : base(message)
        { }

        public TaskLoadFailedException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
