using System;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace DotNet.Basics.Serilog
{
    public static class LoggingExtensions
    {
        public static LogEventLevel ToSeriLogEventLevel(this LogLevel lvl)
        {
            switch (lvl)
            {
                case LogLevel.None:
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                default:
                    throw new ArgumentException($"Level unknown: {lvl}");
            }
        }
    }
}
