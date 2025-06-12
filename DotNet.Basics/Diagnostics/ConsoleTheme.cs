using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public abstract class ConsoleTheme(ConsoleColorSet defaultColors, ConsoleColorSet highlightColors)
    {
        public ConsoleColorSet DefaultColors { get; } = defaultColors;
        public ConsoleColorSet HighlightColors { get; } = highlightColors;

        public void ResetColors()
        {
            Console.ResetColor();
            SetColors(LogLevel.Information, false, false);
        }

        public void SetColors(LogLevel lvl, bool isHighlight, bool isSuccess)
        {
            var colors = isHighlight ? GetHighlightColors(lvl, isSuccess) : GetDefaultColors(lvl, isSuccess);
            Console.ForegroundColor = colors.Foreground;
            Console.BackgroundColor = colors.Background;
        }

        private (ConsoleColor Foreground, ConsoleColor Background) GetDefaultColors(LogLevel lvl, bool isSuccess)
        {
            return isSuccess ? DefaultColors.Success : DefaultColors[lvl];
        }
        private (ConsoleColor Foreground, ConsoleColor Background) GetHighlightColors(LogLevel lvl, bool isSuccess)
        {
            return isSuccess ? HighlightColors.Success : HighlightColors[lvl];
        }
    }
}
