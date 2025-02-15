using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Serilog.Looging
{
    public class Loog : ILoog
    {
        public delegate void MessageLoggedEventHandler(LoogLevel level, string message, Exception? e);
        public delegate void TimingLoggedEventHandler(LoogLevel level, string name, string @event, TimeSpan duration);
        public event MessageLoggedEventHandler? MessageLogged;
        public event TimingLoggedEventHandler? TimingLogged;

        private readonly ConcurrentStack<string> _context;

        public string Context { get; }

        public Loog(params IEnumerable<string> context)
        {
            _context = new(context);
            if (_context.Any())
                Context = string.Join(" / ", _context.Reverse()) + " / ";
            else
                Context = string.Empty;
        }

        public ILoog WithLogTarget(ILoogTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            MessageLogged += target.LogTarget.Invoke;
            TimingLogged += target.TimingTarget.Invoke;
            return this;
        }

        public virtual ILoog InContext(string context, bool floatMessageLogged = true)
        {
            var newLogger = string.IsNullOrWhiteSpace(context)
                ? new Loog(_context)
                : new Loog(_context.Append(context));
            if (floatMessageLogged)
            {
                newLogger.MessageLogged += (lvl, msg, e) => MessageLogged?.Invoke(lvl, msg, e);
                newLogger.TimingLogged += (lvl, name, @event, duration) => TimingLogged?.Invoke(lvl, name, @event, duration);
            }

            return newLogger;
        }
        public void Raw(string message)
        {
            Write(LoogLevel.Raw, message);
        }
        public void Raw(string message, Exception e)
        {
            Write(LoogLevel.Raw, message, e);
        }
        public void Verbose(string message)
        {
            Write(LoogLevel.Verbose, message);
        }
        public void Verbose(string message, Exception e)
        {
            Write(LoogLevel.Verbose, message, e);
        }
        public void Debug(string message)
        {
            Write(LoogLevel.Debug, message);
        }
        public void Debug(string message, Exception e)
        {
            Write(LoogLevel.Debug, message, e);
        }
        public void Info(string message)
        {
            Write(LoogLevel.Info, message);
        }
        public void Info(string message, Exception e)
        {
            Write(LoogLevel.Info, message, e);
        }
        public void Success(string message)
        {
            Write(LoogLevel.Success, message);
        }
        public void Success(string message, Exception e)
        {
            Write(LoogLevel.Success, message, e);
        }
        public void Warning(string message)
        {
            Write(LoogLevel.Warning, message);
        }
        public void Warning(string message, Exception e)
        {
            Write(LoogLevel.Warning, message, e);
        }
        public void Error(string message)
        {
            Write(LoogLevel.Error, message);
        }
        public void Error(string message, Exception e)
        {
            Write(LoogLevel.Error, message, e);
        }

        public void Fatal(string message)
        {
            Write(LoogLevel.Fatal, message);
        }

        public void Fatal(string message, Exception e)
        {
            Write(LoogLevel.Fatal, message, e);
        }

        public void Write(LoogLevel level, string message)
        {
            Write(level, message, null);
        }
        public virtual void Write(LoogLevel level, string message, Exception? e)
        {
            MessageLogged?.Invoke(level, level == LoogLevel.Raw ? message : $"{Context}{message}", e);
        }

        public void Timing(LoogLevel level, string name, string @event)
        {
            Timing(level, name, @event, TimeSpan.MinValue);
        }

        public void Timing(LoogLevel level, string name, string @event, TimeSpan duration)
        {
            TimingLogged?.Invoke(level, name, @event, duration);
        }

        public override string ToString()
        {
            return $"{nameof(Context)}: {Context}";
        }
    }
}
