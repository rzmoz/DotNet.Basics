using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics.Console
{
    public abstract class ConsoleLogTarget : ILogTarget
    {
        protected object SyncRoot { get; } = new object();
        private const string _space = " ";

        public virtual void Write(LogLevel level, string message, Exception e = null)
        {
            lock (SyncRoot)
            {
                var output = FormatLogOutput(level, message, e).StripHighlight();

                if (level < LogLevel.Error)
                    System.Console.Out.Write(output);
                else
                    System.Console.Error.Write(output);
                System.Console.Out.Flush();
            }
        }

        public Action<LogLevel, string, Exception> LogTarget => Write;
        public Action<LogLevel, string, string, TimeSpan> TimingTarget => (level, name, @event, duration) =>
        {
            var durationString = duration > TimeSpan.MinValue ? $" in {duration.ToString("hh\\:mm\\:ss").Highlight()}" : string.Empty;
            Write(level, $"[{name.Highlight()} {@event}{durationString}]".WithGutter());
        };

        protected virtual string FormatLogOutput(LogLevel level, string message, Exception e = null)
        {
            return (FormatLogLevel(level) + FormatMessage(message, e) + Environment.NewLine);
        }

        protected virtual string FormatLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Raw:
                    return string.Empty;
                case LogLevel.Verbose:
                    return "VRB" + _space;
                case LogLevel.Debug:
                    return "DBG" + _space;
                case LogLevel.Info:
                    return "INF" + _space;
                case LogLevel.Success:
                    return "SUC" + _space;
                case LogLevel.Warning:
                    return "WRN" + _space;
                case LogLevel.Error:
                    return "ERR" + _space;
                default:
                    return $"[{level.ToName().ToUpperInvariant()}]" + _space;
            }
        }

        protected virtual string FormatMessage(string message, Exception e = null)
        {
            if (e == null || e is IConsoleException cli && cli.ConsoleLogOptions == ConsoleLogOptions.ExcludeStackTrace)
                return message;

            var exceptionMessage = e.ToString();
            return exceptionMessage.StartsWith($"{e.GetType().FullName}: {message}")
                ? e.ToString()
                : $"{message}\r\n{e}";
        }
    }
}