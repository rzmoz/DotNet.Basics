using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Cli
{
    public class CliException : Exception
    {
        public CliException(bool ignoreStackTraceInLogOutput)
        {
            IgnoreStackTraceInLogOutput = ignoreStackTraceInLogOutput;
        }

        protected CliException(SerializationInfo info, StreamingContext context, bool ignoreStackTraceInLogOutput) : base(info, context)
        {
            IgnoreStackTraceInLogOutput = ignoreStackTraceInLogOutput;
        }

        public CliException(string message, bool ignoreStackTraceInLogOutput) : base(message)
        {
            IgnoreStackTraceInLogOutput = ignoreStackTraceInLogOutput;
        }

        public CliException(string message, Exception innerException, bool ignoreStackTraceInLogOutput) : base(message, innerException)
        {
            IgnoreStackTraceInLogOutput = ignoreStackTraceInLogOutput;
        }

        public bool IgnoreStackTraceInLogOutput { get; set; }
    }
}
