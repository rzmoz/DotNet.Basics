using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Cli
{
    public class CliException : Exception
    {
        public CliException(string message, int exitCode = 500, LogOptions logOptions = LogOptions.ExcludeStackTrace)
            : this(message, null, exitCode, logOptions)
        {
        }

        protected CliException(SerializationInfo info, StreamingContext context, int exitCode, LogOptions logOptions)
            : base(info, context)
        {
            ExitCode = exitCode;
            LogOptions = logOptions;
        }

        public CliException(string message, Exception innerException, int exitCode, LogOptions logOptions)
            : base(message, innerException)
        {
            ExitCode = exitCode;
            LogOptions = logOptions;
        }

        public int ExitCode { get; set; }
        public LogOptions LogOptions { get; set; }
    }
}
