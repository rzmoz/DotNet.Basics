/*using System;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Log = Serilog.Log;

namespace DotNet.Basics.Serilog
{
    public static class LoggingExtensions
    {
        public static LogDispatcher WithSerilog(this LogDispatcher logger, Action<LoggerConfiguration> writeTo = null, LogLevel minimumLevel = LogLevel.Trace)
        {
            if (minimumLevel == LogLevel.None)
                return logger;
            logger.MessageLogged += (level, message, e) => Log.Logger.Write(level.ToSeriLogEventLevel(), e, message);

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(minimumLevel.ToSeriLogEventLevel()));

            writeTo?.Invoke(loggerConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();

            return logger;
        }

        public static LogEventLevel ToSeriLogEventLevel(this LogLevel lvl)
        {
            switch (lvl)
            {
                case LogLevel.None:
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Info:
                    return LogEventLevel.Info;
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
*/