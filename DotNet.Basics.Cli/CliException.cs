using System;
using System.Runtime.Serialization;
using DotNet.Basics.Diagnostics.Console;

namespace DotNet.Basics.Cli
{
    public class CliException : Exception, IConsoleException
    {
        public CliException(string message, int exitCode = 500, ConsoleLogOptions consoleLogOptions = ConsoleLogOptions.ExcludeStackTrace)
            : this(message, null, exitCode, consoleLogOptions)
        {
        }

        protected CliException(SerializationInfo info, StreamingContext context, int exitCode, ConsoleLogOptions consoleLogOptions)
            : base(info, context)
        {
            ExitCode = exitCode;
            ConsoleLogOptions = consoleLogOptions;
        }

        public CliException(string message, Exception innerException, int exitCode, ConsoleLogOptions consoleLogOptions)
            : base(message, innerException)
        {
            ExitCode = exitCode;
            ConsoleLogOptions = consoleLogOptions;
        }

        public int ExitCode { get; set; }
        public ConsoleLogOptions ConsoleLogOptions { get; set; }
    }
}
