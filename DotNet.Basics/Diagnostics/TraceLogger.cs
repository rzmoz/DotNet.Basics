using System;
using System.Diagnostics;

namespace DotNet.Basics.Diagnostics
{
    public class TraceLogger : DotNetBasicsLogger
    {
        protected override void Log(LogEntry entry)
        {
            Trace.WriteLine(FullFormat(entry));
        }
    }
}
