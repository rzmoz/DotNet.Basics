using System;
using System.IO;
using DotNet.Basics.Sys;
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
                var replaceValue = ConsoleMarkers.HighlightPrefix + prop.Value.ToString().Trim('"') + ConsoleMarkers.HighlightSuffix;
                msg = msg.Replace(key, replaceValue, StringComparison.CurrentCulture);
            }

            var isSuccess = logEvent.MessageTemplate.Text.StartsWith(ConsoleMarkers.SuccessPrefix);

            if (isSuccess)
                msg = msg.RemovePrefix(ConsoleMarkers.SuccessPrefix);

            theme.ResetColors();
            theme.SetColors(logEvent.Level, false, isSuccess);
            foreach (var @char in msg)
            {
                switch (@char)
                {
                    case ConsoleMarkers.HighlightPrefix:
                        theme.SetColors(logEvent.Level, true, isSuccess);
                        continue;
                    case ConsoleMarkers.HighlightSuffix:
                        theme.SetColors(logEvent.Level, false, isSuccess);
                        continue;
                    default:
                        output.Write(@char);
                        break;
                }
            }
            theme.ResetColors();
            output.WriteLine(" ");
        }
    }
}