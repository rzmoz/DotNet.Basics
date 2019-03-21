using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class PipelineDispatchException : Exception
    {
        public PipelineDispatchException()
        {
        }

        protected PipelineDispatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PipelineDispatchException(string message) : base(message)
        {
        }

        public PipelineDispatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
