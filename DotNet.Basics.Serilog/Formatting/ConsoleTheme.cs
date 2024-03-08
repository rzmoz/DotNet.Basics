using Serilog.Events;
using System;

namespace DotNet.Basics.Serilog.Formatting
{
    public abstract class ConsoleTheme(ConsoleColorSet foregroundColors, ConsoleColorSet foregroundHighlightColors)
    {
        public ConsoleColorSet ForegroundColors { get; } = foregroundColors;
        public ConsoleColorSet ForegroundHighlightColors { get; } = foregroundHighlightColors;
        
        public ConsoleColor GetForegroundColor(LogEventLevel lvl)
        {
            return ForegroundColors[lvl];
        }
        public ConsoleColor GetForegroundHighlightColor(LogEventLevel lvl)
        {
            return ForegroundHighlightColors[lvl];
        }
    }
}
