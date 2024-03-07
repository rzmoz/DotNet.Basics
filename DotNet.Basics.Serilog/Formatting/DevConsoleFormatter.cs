using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace DotNet.Basics.Serilog.Formatting
{
    public class DevConsoleFormatter : ITextFormatter
    {
        private const char _beginMarker = '\u0002';
        private const char _endMarker = '\u0003';

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var msg = logEvent.MessageTemplate.Text;
            foreach (var prop in logEvent.Properties)
            {
                var key = $"{{{prop.Key}}}";
                var replaceValue = _beginMarker + prop.Value.ToString().Trim('"') + _endMarker;
                msg = msg.Replace(key, replaceValue, StringComparison.CurrentCulture);
            }

            Console.ForegroundColor = GetColor(logEvent.Level);
            foreach (var @char in msg)
            {
                switch (@char)
                {
                    case _beginMarker:
                        Console.ForegroundColor = GetHighlightColor(logEvent.Level);
                        continue;
                    case _endMarker:
                        Console.ForegroundColor = GetColor(logEvent.Level);
                        continue;
                    default:
                        output.Write(@char);
                        break;
                }
            }
            output.Write(Environment.NewLine);
            Console.ResetColor();
        }
        private static ConsoleColor GetHighlightColor(LogEventLevel lvl)
        {
            switch (lvl)
            {
                case LogEventLevel.Verbose:
                    return ConsoleColor.DarkYellow;
                case LogEventLevel.Debug:
                    return ConsoleColor.Gray;
                case LogEventLevel.Information:
                    return ConsoleColor.Cyan;
                case LogEventLevel.Warning:
                    return ConsoleColor.Red;
                case LogEventLevel.Error:
                    return ConsoleColor.White;
                case LogEventLevel.Fatal:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }
        private static ConsoleColor GetColor(LogEventLevel lvl)
        {
            switch (lvl)
            {
                case LogEventLevel.Verbose:
                    return ConsoleColor.DarkGray;
                case LogEventLevel.Debug:
                    return ConsoleColor.DarkCyan;
                case LogEventLevel.Information:
                    return ConsoleColor.White;
                case LogEventLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogEventLevel.Error:
                    return ConsoleColor.Red;
                case LogEventLevel.Fatal:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
