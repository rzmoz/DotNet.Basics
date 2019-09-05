using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class ConsoleFormatter : IConsoleFormatter

    {
        public virtual void FormatLog(LogLevel level, string message, Exception e)
        {
            throw new NotImplementedException();
        }

        public virtual void FormatTiming(string name, string @event, TimeSpan duration)
        {
            throw new NotImplementedException();
        }
    }
}
