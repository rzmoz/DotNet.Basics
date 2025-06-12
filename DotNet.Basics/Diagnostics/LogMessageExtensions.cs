using System;

namespace DotNet.Basics.Diagnostics
{
    public static class LogMessageExtensions
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
    public static class ConsoleMarkers
    {
        public static readonly char HighlightPrefix = '\u0086';
        public static readonly char HighlightSuffix = '\u0087';
        public static readonly string SuccessPrefix = "##[section]";
    }
}
