using Microsoft.Extensions.Logging;
using System;

namespace DotNet.Basics.Diagnostics
{
    public class VoidLogger : ILogger
    {
        public static readonly ILogger Instance = new VoidLogger();
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }
    public class NullLogger : ILogger
    {
        public static readonly ILogger Instance = VoidLogger.Instance;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }
}
