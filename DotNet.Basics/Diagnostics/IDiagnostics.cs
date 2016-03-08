using System;

namespace DotNet.Basics.Diagnostics
{
    public interface IDiagnostics
    {
        void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null);
        void Metric(string name, double value);
        void Profile(string taskName, TimeSpan duration);
    }
}

