using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public abstract class ConsoleWriter : IDiagnosticsTarget
    {
        public abstract void Write(LogLevel level, string message, Exception e = null);
        public Action<LogLevel, string, Exception> LogTarget => Write;
        public Action<string, string, TimeSpan> TimingTarget => (name, @event, duration) =>
        {
            Write(LogLevel.Info,
                $"[{name.Highlight()} {@event} in {duration.ToString("hh\\:mm\\:ss").Highlight()}]".WithGutter());
        };
    }
}
