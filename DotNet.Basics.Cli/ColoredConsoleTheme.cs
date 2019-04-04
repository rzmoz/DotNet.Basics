using System;

namespace DotNet.Basics.Cli
{
    public class ColoredConsoleTheme
    {
        public ConsoleColor VerboseForegroundColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor VerboseBackgroundColor { get; set; }

        public ConsoleColor DebugForegroundColor { get; set; } = ConsoleColor.DarkCyan;
        public ConsoleColor DebugBackgroundColor { get; set; }

        public ConsoleColor InformationVerboseForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor InformationBackgroundColor { get; set; }

        public ConsoleColor WarningForegroundColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor WarningBackgroundColor { get; set; }

        public ConsoleColor ErrorForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ErrorBackgroundColor { get; set; } = ConsoleColor.DarkRed;

        public ConsoleColor CriticalForegroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor CriticalBackgroundColor { get; set; } = ConsoleColor.Red;
    }
}
