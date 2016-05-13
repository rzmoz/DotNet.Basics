using System;
using System.Diagnostics;

namespace DotNet.Basics.Diagnostics
{
    /// <summary>
    /// Logs to Debug. Only useful in local debug and testing scenarios as code needs to run in debug compile
    /// </summary>
    public class DebugLogger : DotNetBasicsLogger
    {
        protected override void Log(LogEntry entry)
        {
            Debug.WriteLine($"{entry.Level}: {entry.Timestamp.ToString("yyyy/MM/dd_hh:mm:ss")} - {entry.Message}{Environment.NewLine}{entry.Exception}");
        }
    }
}
