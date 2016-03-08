using System;
using System.Runtime.Serialization;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public class NoStopConditionIsSetException : Exception
    {
        public NoStopConditionIsSetException()
        {
        }

        public NoStopConditionIsSetException(string message)
            : base(message)
        {
        }

        public NoStopConditionIsSetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoStopConditionIsSetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
