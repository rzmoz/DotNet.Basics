using System;
using System.Runtime.Serialization;

namespace DotNet.Standard.Tasks.Pipelines
{
    public class LazyLoadTaskFailedToLoadException : Exception
    {
        public LazyLoadTaskFailedToLoadException()
        { }

        protected LazyLoadTaskFailedToLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public LazyLoadTaskFailedToLoadException(string message) : base(message)
        { }

        public LazyLoadTaskFailedToLoadException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
