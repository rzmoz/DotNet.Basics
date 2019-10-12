using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Cli
{
    public class CliException : Exception
    {
        public CliException(LogOptions logOptions)
        {
            LogOptions = logOptions;
        }

        protected CliException(SerializationInfo info, StreamingContext context, LogOptions logOptions) : base(info, context)
        {
            LogOptions = logOptions;
        }

        public CliException(string message, LogOptions logOptions) : base(message)
        {
            LogOptions = logOptions;
        }

        public CliException(string message, Exception innerException, LogOptions logOptions) : base(message, innerException)
        {
            LogOptions = logOptions;
        }

        public LogOptions LogOptions { get; set; }
    }
}
