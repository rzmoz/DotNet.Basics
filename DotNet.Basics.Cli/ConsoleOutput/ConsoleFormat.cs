using System.Drawing;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class ConsoleFormat
    {
        public ConsoleFormat()
            : this(AnsiForegroundColor.Empty)
        { }

        public ConsoleFormat(Color foregroundColor)
            : this(new AnsiForegroundColor(foregroundColor), AnsiBackgroundColor.Empty)
        { }

        public ConsoleFormat(Color foregroundColor, Color backgroundColor)
            : this(new AnsiForegroundColor(foregroundColor), new AnsiBackgroundColor(backgroundColor), AnsiForegroundColor.Empty)
        { }

        public ConsoleFormat(Color foregroundColor, Color backgroundColor, Color highlightForegroundColor)
            : this(new AnsiForegroundColor(foregroundColor), new AnsiBackgroundColor(backgroundColor), new AnsiForegroundColor(highlightForegroundColor), AnsiBackgroundColor.Empty)
        { }

        public ConsoleFormat(Color foregroundColor, Color backgroundColor, Color highlightForegroundColor, Color highlightBackgroundColor)
        : this(new AnsiForegroundColor(foregroundColor), new AnsiBackgroundColor(backgroundColor), new AnsiForegroundColor(highlightForegroundColor), new AnsiBackgroundColor(highlightBackgroundColor))
        { }

        public ConsoleFormat(AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor = null, AnsiForegroundColor highlightForegroundColor = null, AnsiBackgroundColor highlightBackgroundColor = null)
        {
            ForegroundColor = foregroundColor ?? AnsiForegroundColor.Empty;
            BackgroundColor = backgroundColor ?? AnsiBackgroundColor.Empty;
            HighlightForegroundColor = highlightForegroundColor ?? AnsiForegroundColor.Empty;
            HighlightBackgroundColor = highlightBackgroundColor ?? AnsiBackgroundColor.Empty;
        }

        public static ConsoleFormat Empty => new ConsoleFormat(AnsiForegroundColor.Empty);

        public AnsiForegroundColor ForegroundColor { get; }
        public AnsiBackgroundColor BackgroundColor { get; }
        public AnsiForegroundColor HighlightForegroundColor { get; }
        public AnsiBackgroundColor HighlightBackgroundColor { get; }
    }
}
