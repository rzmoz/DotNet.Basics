using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public static class LogExtensions
    {
        public static string HighlightPrefix { get; } = "<¤>";
        public static string HighlightSuffix { get; } = "</¤>]";

        public static string Highlight(this string str)
        {
            return str.EnsurePrefix(HighlightPrefix).EnsureSuffix(HighlightSuffix);
        }
        public static string StripHighlight(this string str)
        {
            return str.Replace(HighlightPrefix, string.Empty).Replace(HighlightSuffix, string.Empty);
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
