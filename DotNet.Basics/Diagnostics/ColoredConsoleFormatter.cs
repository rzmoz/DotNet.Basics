using Microsoft.Extensions.Logging;
using System.IO;

namespace DotNet.Basics.Diagnostics
{
    public class ColoredConsoleFormatter(ConsoleTheme theme) : ILogFormatter
    {
        public ColoredConsoleFormatter()
        : this(new ConsoleDarkTheme())
        { }

        public void Format(LogLevel level, string? message, TextWriter output)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var isSuccess = message.IsSuccess();
            message = message.StripSuccess();

            theme.ResetColors();
            theme.SetColors(level, false, isSuccess);
            foreach (var @char in message)
            {
                switch (@char)
                {
                    case ConsoleMarkers.HighlightPrefix:
                        theme.SetColors(level, true, isSuccess);
                        continue;
                    case ConsoleMarkers.HighlightSuffix:
                        theme.SetColors(level, false, isSuccess);
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
