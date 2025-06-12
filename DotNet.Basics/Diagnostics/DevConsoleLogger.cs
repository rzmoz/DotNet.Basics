using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace DotNet.Basics.Diagnostics
{
    public class DevConsoleLogger(DevConsoleOptions consoleOptions) : ILogger
    {
        private static readonly object _messageLock = new();

        private readonly ILogFormatter _formatter = consoleOptions.IsAdo ? new AdoPipelinesLogFormatter() : new ColoredConsoleFormatter();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var output = GetOutStream(logLevel);

            var message = formatter?.Invoke(state, exception);

            lock (_messageLock)
            {
                _formatter.Format(logLevel, message, output);
                output.Flush();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= consoleOptions.LogLevel;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        private TextWriter GetOutStream(LogLevel level)
        {
            return level < LogLevel.Error ? Console.Out : Console.Error;
        }
    }
}
