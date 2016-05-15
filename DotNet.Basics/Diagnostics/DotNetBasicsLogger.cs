using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public abstract class DotNetBasicsLogger : ILogger
    {
        protected DotNetBasicsLogger()
        {
            MinimumLevel = LogLevel.Debug;
        }

        public LogLevel MinimumLevel { get; set; }

        public virtual void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) == false)
                return;
            var message = formatter != null ? formatter(state, exception) : LogFormatter.Formatter(state, exception);
            if (string.IsNullOrEmpty(message))
                return;
            Log(new LogEntry(GetTimestamp(), message, logLevel, exception));
        }

        protected abstract void Log(LogEntry entry);

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return MinimumLevel <= logLevel && logLevel != LogLevel.None;
        }

        public virtual IDisposable BeginScopeImpl(object state)
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
