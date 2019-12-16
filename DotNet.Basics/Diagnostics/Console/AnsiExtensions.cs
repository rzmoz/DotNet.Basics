using System.Linq;
using System.Text;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Diagnostics.Console;

namespace DotNet.Basics.Diagnostics.Console
{
    public static class AnsiExtensions
    {
        public static string AnsiColorize(this string text, AnsiColor color)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var ansi = new StringBuilder();
            ansi.Append(color.AnsiCode);
            ansi.Append(text);
            ansi.Append(AnsiColor.ResetString);
            return ansi.ToString();
        }

        public static string AnsiColorize(this string text, ConsoleFormat format)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var colorsAnsiCode = ToAnsiCode(format.ForegroundColor, format.BackgroundColor);
            var highlightAnsiCode = ToAnsiCode(format.HighlightForegroundColor, format.HighlightBackgroundColor);
            text = text.Replace(LogExtensions.HighlightPrefix, highlightAnsiCode).Replace(LogExtensions.HighlightSuffix, AnsiColor.ResetString + colorsAnsiCode);
            var ansi = new StringBuilder();

            ansi.Append(colorsAnsiCode);
            ansi.Append(text);
            ansi.Append(AnsiColor.ResetString);
            return ansi.ToString();
        }

        internal static string ToAnsiCode(params AnsiColor[] colors)
        {
            return string.Join(string.Empty, colors.Select(color => color.AnsiCode));
        }
    }
}
