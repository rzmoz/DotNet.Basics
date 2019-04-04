using System.Drawing;
using System.Linq;
using System.Text;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public static class AnsiExtensions
    {
        private const string _highlightPrefix = "[!]";
        private const string _highlightSuffix = "[/]";

        private const string _ansiEscapeCode = "\u001b[";
        private const string _ansiTermination = "m";
        private const string _ansiReset = _ansiEscapeCode + "0" + _ansiTermination;

        public static string AnsiHighlight(this string str)
        {
            return str.EnsurePrefix(_highlightPrefix).EnsureSuffix(_highlightSuffix);
        }

        public static string AnsiColorize(this string text, ConsoleFormat format)
        {
            return text.AnsiColorize(format.ForegroundColor, format.BackgroundColor);
        }
        
        public static string AnsiColorize(this string text, params AnsiColor[] colors)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var ansi = new StringBuilder();
            ansi.Append(string.Join("", colors.Where(color => color.Color != Color.Empty).Select(color => $"{_ansiEscapeCode}{color.ColorString}{_ansiTermination}")));
            
            ansi.Append(text);
            ansi.Append(_ansiReset);
            return ansi.ToString();
        }
    }
}
