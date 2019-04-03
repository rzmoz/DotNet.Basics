using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogDispatcher : IDisposable
    {
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception e);
        public event MessageLoggedEventHandler MessageLogged;

        public delegate void CloseAndFlushEventHandler();
        public event CloseAndFlushEventHandler ClosingAndFlushing;

        public void CloseAndFlush()
        {
            ClosingAndFlushing?.Invoke();
        }

        public void Verbose(string message)
        {
            Verbose(message, null);
        }
        public void Verbose(string message, Exception e)
        {
            Write(LogLevel.Trace, message, e);
        }
        public void Debug(string message)
        {
            Debug(message, null);
        }
        public void Debug(string message, Exception e)
        {
            Write(LogLevel.Debug, message, e);
        }
        public void Information(string message)
        {
            Information(message, null);
        }
        public void Information(string message, Exception e)
        {
            Write(LogLevel.Information, message, e);
        }
        public void Warning(string message)
        {
            Warning(message, null);
        }
        public void Warning(string message, Exception e)
        {
            Write(LogLevel.Warning, message, e);
        }
        public void Error(string message)
        {
            Error(message, null);
        }
        public void Error(string message, Exception e)
        {
            Write(LogLevel.Error, message, e);
        }
        public void Critical(string message)
        {
            Critical(message, null);
        }
        public void Critical(string message, Exception e)
        {
            Write(LogLevel.Critical, message, e);
        }

        public void Write(LogLevel level, string message, Exception e)
        {
            MessageLogged?.Invoke(level, message, e);
        }

        public void Dispose()
        {
            CloseAndFlush();
        }
    }
}
