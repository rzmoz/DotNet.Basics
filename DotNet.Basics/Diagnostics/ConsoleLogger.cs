using System;

namespace DotNet.Basics.Diagnostics
{
    public class ConsoleLogger : DotNetBasicsLogger

    {
        protected override void Log(LogEntry entry)
        {
            Console.WriteLine($"{entry.Level}: {entry.Timestamp.ToString("yyyy/MM/dd_hh:mm:ss")} - {entry.Message}{Environment.NewLine}{entry.Exception}");
        }
    }
}
