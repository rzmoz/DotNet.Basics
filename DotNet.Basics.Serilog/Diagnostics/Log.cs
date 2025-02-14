using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Serilog.Diagnostics
{
    public class Log : ILog
    {
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception? e);
        public delegate void TimingLoggedEventHandler(LogLevel level, string name, string @event, TimeSpan duration);
        public event MessageLoggedEventHandler? MessageLogged;
        public event TimingLoggedEventHandler? TimingLogged;

        private readonly ConcurrentStack<string> _context;

        public string Context { get; }

        public Log(params IEnumerable<string> context)
        {
            _context = new(context);
            if (_context.Any())
                Context = string.Join(" / ", _context.Reverse()) + " / ";
            else
                Context = string.Empty;
        }

        public ILog WithLogTarget(ILogTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            MessageLogged += target.LogTarget.Invoke;
            TimingLogged += target.TimingTarget.Invoke;
            return this;
        }

        public virtual ILog InContext(string context, bool floatMessageLogged = true)
        {
            var newLogger = string.IsNullOrWhiteSpace(context)
                ? new Log(_context)
                : new Log(_context.Append(context));
            if (floatMessageLogged)
            {
                newLogger.MessageLogged += (lvl, msg, e) => MessageLogged?.Invoke(lvl, msg, e);
                newLogger.TimingLogged += (lvl, name, @event, duration) => TimingLogged?.Invoke(lvl, name, @event, duration);
            }

            return newLogger;
        }
        public void Raw(string message)
        {
            Write(LogLevel.Raw, message);
        }
        public void Raw(string message, Exception e)
        {
            Write(LogLevel.Raw, message, e);
        }
        public void Verbose(string message)
        {
            Write(LogLevel.Verbose, message);
        }
        public void Verbose(string message, Exception e)
        {
            Write(LogLevel.Verbose, message, e);
        }
        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }
        public void Debug(string message, Exception e)
        {
            Write(LogLevel.Debug, message, e);
        }
        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }
        public void Info(string message, Exception e)
        {
            Write(LogLevel.Info, message, e);
        }
        public void Success(string message)
        {
            Write(LogLevel.Success, message);
        }
        public void Success(string message, Exception e)
        {
            Write(LogLevel.Success, message, e);
        }
        public void Warning(string message)
        {
            Write(LogLevel.Warning, message);
        }
        public void Warning(string message, Exception e)
        {
            Write(LogLevel.Warning, message, e);
        }
        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }
        public void Error(string message, Exception e)
        {
            Write(LogLevel.Error, message, e);
        }
        public void Write(LogLevel level, string message)
        {
            Write(level, message, null);
        }
        public virtual void Write(LogLevel level, string message, Exception? e)
        {
            MessageLogged?.Invoke(level, level == LogLevel.Raw ? message : $"{Context}{message}", e);
        }

        public void Timing(LogLevel level, string name, string @event)
        {
            Timing(level, name, @event, TimeSpan.MinValue);
        }

        public void Timing(LogLevel level, string name, string @event, TimeSpan duration)
        {
            TimingLogged?.Invoke(level, name, @event, duration);
        }

        public override string ToString()
        {
            return $"{nameof(Context)}: {Context}";
        }
    }
}
