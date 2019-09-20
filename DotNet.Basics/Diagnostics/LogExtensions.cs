using System.Linq;
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

        public static string WithGutter(this string msg)
        {
            return msg.WithIndent(26);
        }
        public static string WithIndent(this string msg, int indent = 4)
        {
            var indentString = string.Join("", Enumerable.Range(0, indent).Select(r => " ").ToArray());
            return indentString + msg.Replace("\r\n", $"\r\n{indentString}");
        }
    }
}
