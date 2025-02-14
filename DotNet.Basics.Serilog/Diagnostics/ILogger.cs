using System;

namespace DotNet.Basics.Serilog.Diagnostics
{
    public interface ILogger : ILogDispatcher
    {
        ILogger WithLogTarget(ILogTarget target);

        ILogger InContext(string context, bool floatMessageLogged = true);
        void Raw(string message);
        void Raw(string message, Exception e);
        void Verbose(string message);
        void Verbose(string message, Exception e);
        void Debug(string message);
        void Debug(string message, Exception e);
        void Info(string message);
        void Info(string message, Exception e);
        void Success(string message);
        void Success(string message, Exception e);
        void Warning(string message);
        void Warning(string message, Exception e);
        void Error(string message);
        void Error(string message, Exception e);
        void Write(LogLevel level, string message);
        void Write(LogLevel level, string message, Exception e);
        void Timing(LogLevel level, string name, string @event);
        void Timing(LogLevel level, string name, string @event, TimeSpan duration);
    }
}
