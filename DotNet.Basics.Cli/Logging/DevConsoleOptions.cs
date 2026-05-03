
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleOptions
    {
        public bool IsAdo { get; set; }
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;
    }
}
