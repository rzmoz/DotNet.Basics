using System;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public abstract class ConsoleWriter : IDiagnosticsTarget
    {
        protected object SyncRoot { get; } = new object();
        private const string _space = " ";
        
        public virtual void Write(LogLevel level, string message, Exception e = null)
        {
            lock (SyncRoot)
            {
                var output = FormatLogOutput(level, message, e).StripHighlight();

                if (level < LogLevel.Error)
                    WriteOutput(output);
                else
                    WriteError(output);
                Console.Out.Flush();
            }
        }

        public Action<LogLevel, string, Exception> LogTarget => Write;
        public Action<LogLevel, string, string, TimeSpan> TimingTarget => (level, name, @event, duration) =>
        {
            var durationString = duration > TimeSpan.MinValue ? $" in {duration.ToString("hh\\:mm\\:ss").Highlight()}" : string.Empty;
            Write(level, $"[{name.Highlight()} {@event}{durationString}]".WithGutter());
        };

        protected virtual void WriteOutput(string output)
        {
            Console.Out.Write(output);
        }
        protected virtual void WriteError(string output)
        {
            Console.Error.Write(output);
        }

        protected virtual string FormatLogOutput(LogLevel level, string message, Exception e = null)
        {
            return FormatLogLevel(level) + _space + FormatMessage(message, e) + Environment.NewLine;
        }

        protected virtual string FormatLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return "VRB";
                case LogLevel.Debug:
                    return "DBG";
                case LogLevel.Info:
                    return "INF";
                case LogLevel.Success:
                    return "SUC";
                case LogLevel.Warning:
                    return "WRN";
                case LogLevel.Error:
                    return "ERR";
                default:
                    return $"[{level.ToName().ToUpperInvariant()}]";
            }
        }

        protected virtual string FormatMessage(string message, Exception e = null)
        {
            if (e == null || e is CliException cli && cli.LogOptions == LogOptions.ExcludeStackTrace)
                return message;

            var exceptionMessage = e.ToString();
            return exceptionMessage.StartsWith($"{e.GetType().FullName}: {message}")
                ? e.ToString()
                : $"{message}\r\n{e}";
        }
    }
}