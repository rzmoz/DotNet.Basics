using System;

namespace DotNet.Basics.Diagnostics
{
    public class VoidDiagnostics : IDiagnostics
    {
        public void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null)
        {
            //write to void
        }

        public void Metric(string name, double value)
        {
            //write to void
        }

        public void Profile(string taskName, TimeSpan duration)
        {
            //write to void
        }
    }
}
