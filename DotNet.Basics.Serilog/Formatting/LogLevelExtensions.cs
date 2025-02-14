using DotNet.Basics.Serilog.Diagnostics;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public static class LogLevelExtensions
    {
        public static LogEventLevel ToLogEventLevel(this LogLevel lvl)
        {
            return lvl switch
            {
                LogLevel.Verbose => LogEventLevel.Verbose,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Info => LogEventLevel.Information,
                LogLevel.Success => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Raw => LogEventLevel.Information,
                _ => LogEventLevel.Information
            };
        }
    }
}
