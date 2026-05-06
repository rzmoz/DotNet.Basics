using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli.Logging
{
    public interface IConsoleLogger : ILogger
    {
        public LogLevel MinimumLogLevel { get; set; }
    }
}
