using System;
using NLog;

namespace DotNet.Basics.Pipelines
{
    public class PipelineLogger : IPipelineLogger
    {
        public event EventHandler<LogEventInfo> EntryLogged;
        private readonly ILogger _loggerPassThru;

        public PipelineLogger(ILogger loggerPassThru = null)
        {
            _loggerPassThru = loggerPassThru;
        }

        public void Log(LogLevel level, string message, Exception e = null)
        {
            var logEntry = new LogEventInfo(level, string.Empty, message)
            {
                Exception = e,
                TimeStamp = DateTime.UtcNow
            };

            EntryLogged?.Invoke(null, logEntry);
            _loggerPassThru?.Log(logEntry);
        }

        public void Trace(string message, Exception e = null)
        {
            Log(LogLevel.Trace, message, e);
        }

        public void Debug(string message, Exception e = null)
        {
            Log(LogLevel.Debug, message, e);
        }

        public void Info(string message, Exception e = null)
        {
            Log(LogLevel.Info, message, e);
        }

        public void Warn(string message, Exception e = null)
        {
            Log(LogLevel.Warn, message, e);
        }

        public void Error(string message, Exception e = null)
        {
            Log(LogLevel.Error, message, e);
        }

        public void Fatal(string message, Exception e = null)
        {
            Log(LogLevel.Fatal, message, e);
        }
    }
}
