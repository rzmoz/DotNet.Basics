using System;

namespace DotNet.Basics.Serilog.Diagnostics
{
    public class NullLogger : ILogger
    {
        public event Logger.MessageLoggedEventHandler? MessageLogged;
        public event Logger.TimingLoggedEventHandler? TimingLogged;

        public static ILogger Instance { get; } = new NullLogger();

        public ILogger WithLogTarget(ILogTarget target)
        {
            return this;
        }

        public ILogger InContext(string context, bool floatMessageLogged = true)
        {
            return this;
        }
        public void Raw(string message)
        { }
        public void Raw(string message, Exception e)
        { }
        public void Verbose(string message)
        { }
        public void Verbose(string message, Exception e)
        { }
        public void Debug(string message)
        { }
        public void Debug(string message, Exception e)
        { }
        public void Info(string message)
        { }
        public void Info(string message, Exception e)
        { }
        public void Success(string message)
        { }
        public void Success(string message, Exception e)
        { }
        public void Warning(string message)
        { }
        public void Warning(string message, Exception e)
        { }
        public void Error(string message)
        { }
        public void Error(string message, Exception e)
        { }
        public void Write(LogLevel level, string message)
        { }
        public void Write(LogLevel level, string message, Exception e)
        { }
        public void Timing(LogLevel level, string name, string @event)
        { }
        public void Timing(LogLevel level, string name, string @event, TimeSpan duration)
        { }
    }
}
