namespace DotNet.Basics.Cli
{
    public class ConsoleFormat
    {
        public ConsoleFormat(AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor = null, AnsiForegroundColor highlightForegroundColor = null)
        {
            ForegroundColor = foregroundColor ?? AnsiForegroundColor.Empty;
            BackgroundColor = backgroundColor ?? AnsiBackgroundColor.Empty;
            HighlightForegroundColor = highlightForegroundColor ?? AnsiForegroundColor.Empty;
        }

        public static ConsoleFormat Empty => new ConsoleFormat(AnsiForegroundColor.Empty);

        public AnsiForegroundColor ForegroundColor { get; }
        public AnsiBackgroundColor BackgroundColor { get; }
        public AnsiForegroundColor HighlightForegroundColor { get; }
    }
}
