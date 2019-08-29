using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogDispatcher : ILogDispatcher
    {
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception e);
        public delegate void MetricLoggedEventHandler(string name, double value);
        public event MessageLoggedEventHandler MessageLogged;
        public event MetricLoggedEventHandler MetricLogged;

        private readonly ConcurrentStack<string> _context;

        public string Context { get; }

        public LogDispatcher(IEnumerable<string> context)
        : this((context ?? new string[0]).ToArray())
        {
        }
        public LogDispatcher(params string[] context)
        {
            _context = new ConcurrentStack<string>(context);
            if (_context.Any())
                Context = string.Join(" / ", _context.Reverse()) + " / ";
            else
                Context = string.Empty;
        }

        public ILogDispatcher InContext(string context, bool floatMessageLogged = true)
        {
            var newLogger = string.IsNullOrWhiteSpace(context)
                ? new LogDispatcher(_context)
                : new LogDispatcher(_context.Append(context));
            if (floatMessageLogged)
            {
                newLogger.MessageLogged += (lvl, msg, e) => MessageLogged?.Invoke(lvl, msg, e);
                newLogger.MetricLogged += (name, value) => MetricLogged?.Invoke(name, value);
            }

            return newLogger;
        }

        public void Metric(string name, double value)
        {
            MetricLogged?.Invoke(name, value);
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
        public void Write(LogLevel level, string message)
        {
            Write(level, message, null);
        }
        public void Write(LogLevel level, string message, Exception e)
        {
            MessageLogged?.Invoke(level, $"{Context}{message}", e);
        }

        public override string ToString()
        {
            return $"{nameof(Context)}: {Context}";
        }
    }
}
