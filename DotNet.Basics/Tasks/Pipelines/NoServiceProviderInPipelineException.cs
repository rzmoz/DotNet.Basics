using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class NoServiceProviderInPipelineException : Exception
    {
        public NoServiceProviderInPipelineException()
        {
        }

        protected NoServiceProviderInPipelineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NoServiceProviderInPipelineException(string message) : base(message)
        {
        }

        public NoServiceProviderInPipelineException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
