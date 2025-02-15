using System;

namespace DotNet.Basics.Serilog.Looging
{
    public static class LoogExtensions
    {
        public static string Highlight(this string str)
        {
            return $"{ConsoleMarkers.HighlightPrefix}{str}{ConsoleMarkers.HighlightSuffix}";
        }
        public static string StripHighlight(this string str)
        {
            return str.Replace($"{ConsoleMarkers.HighlightPrefix}", string.Empty).Replace($"{ConsoleMarkers.HighlightSuffix}", string.Empty);
        }
        public static string WithGutter(this string msg, int gutterSize = 26)
        {
            return msg.WithIndent(gutterSize);
        }
        public static string WithIndent(this string msg, int indent = 4)
        {
            var indentString = new string(' ', indent);
            if (msg.Contains("\r\n"))
                msg = Environment.NewLine + msg;
            return indentString + msg.Replace("\r\n", $"\r\n{indentString}");
        }
    }
}
