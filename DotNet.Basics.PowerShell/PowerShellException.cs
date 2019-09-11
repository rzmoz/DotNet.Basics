using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.PowerShell
{
    public class PowerShellException : Exception
    {
        public PowerShellException(int exitCode)
        {
            ExitCode = exitCode;
        }

        protected PowerShellException(SerializationInfo info, StreamingContext context, int exitCode) : base(info, context)
        {
            ExitCode = exitCode;
        }

        public PowerShellException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public PowerShellException(string message, Exception innerException, int exitCode) : base(message, innerException)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; set; }
    }
}
