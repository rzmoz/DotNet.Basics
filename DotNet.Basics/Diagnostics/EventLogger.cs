using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class EventLogger : ILogger
    {
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception? e);
        public event MessageLoggedEventHandler? MessageLogged;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            MessageLogged?.Invoke(logLevel, formatter.Invoke(state, exception), exception);

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    }
}
