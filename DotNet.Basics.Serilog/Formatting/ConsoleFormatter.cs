using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleFormatter(ConsoleTheme theme) : ITextFormatter
    {
        public ConsoleFormatter() : this(new ConsoleDarkTheme())
        { }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var msg = logEvent.MessageTemplate.Text;
            foreach (var prop in logEvent.Properties)
            {
                var key = $"{{{prop.Key}}}";
                var replaceValue = HighlightMarkers.Prefix + prop.Value.ToString().Trim('"') + HighlightMarkers.Suffix;
                msg = msg.Replace(key, replaceValue, StringComparison.CurrentCulture);
            }
            Console.ResetColor();
            Console.ForegroundColor = theme.GetForegroundColor(logEvent.Level);
            foreach (var @char in msg)
            {
                switch (@char)
                {
                    case HighlightMarkers.Prefix:
                        Console.ForegroundColor = theme.GetForegroundHighlightColor(logEvent.Level);
                        continue;
                    case HighlightMarkers.Suffix:
                        Console.ForegroundColor = theme.GetForegroundColor(logEvent.Level);
                        continue;
                    default:
                        output.Write(@char);
                        break;
                }
            }
            output.Write(Environment.NewLine);
            Console.ResetColor();
            Console.ForegroundColor = theme.GetForegroundColor(logEvent.Level);
        }
    }
}