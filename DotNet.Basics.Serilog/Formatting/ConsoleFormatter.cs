using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleFormatter : ITextFormatter
    {
        private const char _beginMarker = '\u0002';
        private const char _endMarker = '\u0003';

        private readonly ConsoleTheme _theme;

        public ConsoleFormatter()
        : this(new ConsoleDarkTheme())
        { }

        public ConsoleFormatter(ConsoleTheme theme)
        {
            _theme = theme;
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var msg = logEvent.MessageTemplate.Text;
            foreach (var prop in logEvent.Properties)
            {
                var key = $"{{{prop.Key}}}";
                var replaceValue = _beginMarker + prop.Value.ToString().Trim('"') + _endMarker;
                msg = msg.Replace(key, replaceValue, StringComparison.CurrentCulture);
            }

            Console.ForegroundColor = _theme.GetForegroundColor(logEvent.Level);
            foreach (var @char in msg)
            {
                switch (@char)
                {
                    case _beginMarker:
                        Console.ForegroundColor = _theme.GetForegroundHighlightColor(logEvent.Level);
                        continue;
                    case _endMarker:
                        Console.ForegroundColor = _theme.GetForegroundColor(logEvent.Level);
                        continue;
                    default:
                        output.Write(@char);
                        break;
                }
            }
            output.Write(Environment.NewLine);
            Console.ResetColor();
        }
    }
}