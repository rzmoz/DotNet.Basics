using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class DevConsoleOptions
    {
        public bool IsAdo { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;
    }
}
