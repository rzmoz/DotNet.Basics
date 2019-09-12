using System;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public abstract class ConsoleWriter : IDiagnosticsTarget
    {
        public abstract void Write(LogLevel level, string message, Exception e = null);
        public Action<LogLevel, string, Exception> LogTarget => Write;
        public Action<LogLevel, string, string, TimeSpan> TimingTarget => (level, name, @event, duration) =>
        {
            Write(level, $"[{name.Highlight()} {@event} in {duration.ToString("hh\\:mm\\:ss").Highlight()}]".WithGutter());
        };

        protected virtual string ToOutputString(LogLevel level)
        {
            return $"{level.ToName().ToUpperInvariant()}";
        }
    }
}
