using System;

namespace DotNet.Basics.Pipelines
{
    public class PipelineException : Exception
    {
        public PipelineException(int exitCode)
        {
            ExitCode = exitCode;
        }

        public PipelineException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public PipelineException(string message, Exception innerException, int exitCode) : base(message, innerException)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; set; }
    }
}
