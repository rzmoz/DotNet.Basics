using System;
using NLog;

namespace DotNet.Basics.Pipelines
{
    public interface IPipelineLogger
    {
        void Log(LogLevel level, string message, Exception e = null);
        void Trace(string message, Exception e = null);
        void Debug(string message, Exception e = null);
        void Info(string message, Exception e = null);
        void Warn(string message, Exception e = null);
        void Error(string message, Exception e = null);
        void Fatal(string message, Exception e = null);
    }
}
