using System;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleDarkTheme() : ConsoleTheme(GetForegroundColors(), GetForegroundHighlightColors())
    {
        private static ConsoleColorSet GetForegroundColors()
        {
            return new ConsoleColorSet
            {

                { LogEventLevel.Verbose, ConsoleColor.DarkGray},
                { LogEventLevel.Debug, ConsoleColor.DarkCyan},
                { LogEventLevel.Information, ConsoleColor.White },
                { LogEventLevel.Warning, ConsoleColor.Yellow},
                { LogEventLevel.Error, ConsoleColor.Red},
                { LogEventLevel.Fatal, ConsoleColor.Red}
        };
        }
        private static ConsoleColorSet GetForegroundHighlightColors()
        {
            return new ConsoleColorSet
            {
                { LogEventLevel.Verbose, ConsoleColor.DarkYellow},
                { LogEventLevel.Debug, ConsoleColor.Gray},
                { LogEventLevel.Information, ConsoleColor.Cyan},
                { LogEventLevel.Warning, ConsoleColor.Red},
                { LogEventLevel.Error, ConsoleColor.White },
                { LogEventLevel.Fatal, ConsoleColor.White }
            };
        }
    }
}
