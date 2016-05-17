using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public abstract class DotNetBasicsLogger : ILogger
    {
        protected abstract void Log(LogEntry entry);

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) == false)
                return;
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
                return;
            Log(new LogEntry(GetTimestamp(), message, logLevel, exception));
        }

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return null;
        }

        protected virtual DateTime GetTimestamp()
        {
            return DateTime.UtcNow;
        }
    }
}
