using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public interface IConsoleFormatter
    {
        void FormatLog(LogLevel level, string message, Exception e);
        void FormatTiming(string name, string @event, TimeSpan duration);
    }
}
