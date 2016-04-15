using System;

namespace DotNet.Basics.Diagnostics
{
    public class NullDiagnostics : IDiagnostics
    {
        public void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null)
        {
            //log to null
        }

        public void Metric(string name, double value)
        {
            //log to null
        }

        public void Profile(string taskName, TimeSpan duration)
        {
            //log to null
        }
    }
}
