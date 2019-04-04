using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public static class AnsiExtensions
    {
        private const string _highlightPrefix = "[!]";
        private const string _highlightSuffix = "[/]";

        public static string AnsiHighlight(this string str)
        {
            return str.EnsurePrefix(_highlightPrefix).EnsureSuffix(_highlightSuffix);
        }
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
            text = text.Replace(_highlightPrefix, highlightAnsiCode).Replace(_highlightSuffix, AnsiColor.ResetString + colorsAnsiCode);
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
