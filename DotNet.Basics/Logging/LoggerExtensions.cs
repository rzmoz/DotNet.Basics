using System;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Logging
{
    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, string message, LogLevel logLevel, Exception exception = null)
        {
            Log(logger, DateTime.UtcNow, message, logLevel, exception);
        }
        public static void Log(this ILogger logger, DateTime timestamp, string message, LogLevel logLevel, Exception exception = null)
        {
            if (logger.IsEnabled(logLevel) == false)
                return;

            logger.Log(logLevel, 0, message, exception, (state, e) =>
            {
                var output = $"<{logLevel.ToName()}> {message}";
                if (exception != null)
                    output += Environment.NewLine + exception.ToString();

                return $"[{timestamp.ToString("yyyy-MM-dd hh:mm:ss")}] {output}";
            });
        }

    }
}
