using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public interface ILogDispatcher : ILogger
    {
        ILogDispatcher InContext(string context, bool floatMessageLogged = true);
        void Metric(string message, double value);
        void Verbose(string message);
        void Verbose(string message, Exception e);
        void Debug(string message);
        void Debug(string message, Exception e);
        void Information(string message);
        void Information(string message, Exception e);
        void Warning(string message);
        void Warning(string message, Exception e);
        void Error(string message);
        void Error(string message, Exception e);
        void Critical(string message);
        void Critical(string message, Exception e);
        void Write(LogLevel level, string message);
        void Write(LogLevel level, string message, Exception e);
    }
}
