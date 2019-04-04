using System;
using System.Collections.Concurrent;
using System.Linq;
using DotNet.Basics.Collections;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogDispatcher : ILogger, IDisposable
    {
        private readonly object _syncRoot = new object();
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception e);
        public event MessageLoggedEventHandler MessageLogged;

        public delegate void CloseAndFlushEventHandler();
        public event CloseAndFlushEventHandler ClosingAndFlushing;

        private readonly ConcurrentStack<string> _context = new ConcurrentStack<string>();

        public string Context { get; private set; }

        public LogDispatcher PushContext(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
                return this;
            _context.Push(context);
            UpdateContext();
            return this;
        }
        public LogDispatcher PopContext(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
                return this;
            _context.Push(context);
            UpdateContext();
            return this;
        }

        private void UpdateContext()
        {
            if (_context.None())
                return;
            lock (_syncRoot)
            {
                Context = string.Join(" / ", _context.Reverse()) + " / ";
            }
        }

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
            MessageLogged?.Invoke(level, $"{Context}{message}", e);
        }

        public void Dispose()
        {
            CloseAndFlush();
        }

        public override string ToString()
        {
            return $"{nameof(Context)}: {Context}";
        }
    }
}
