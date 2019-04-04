using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public static class LogExtensions
    {
        public static string HighlightPrefix { get; } = "<>";
        public static string HighlightSuffix { get; } = "</>]";

        public static string Highlight(this string str)
        {
            return str.EnsurePrefix(HighlightPrefix).EnsureSuffix(HighlightSuffix);
        }
    }
}
