using System;

namespace DotNet.Basics.Serilog.Diagnostics
{
    public static class LoogExtensions
    {
        public static string Highlight(this string str)
        {
            return $"{HighlightMarkers.Prefix}{str}{HighlightMarkers.Suffix}";
        }
        public static string StripHighlight(this string str)
        {
            return str.Replace($"{HighlightMarkers.Prefix}", string.Empty).Replace($"{HighlightMarkers.Suffix}", string.Empty);
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
