using System.Drawing;
using System.Text;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class AnsiForegroundColor : AnsiColor
    {
        public AnsiForegroundColor(Color color) : base(color, AnsiColorPlane.Foreground)
        {
        }
        public static AnsiForegroundColor Empty => new AnsiForegroundColor(Color.Empty);
    }
    public class AnsiBackgroundColor : AnsiColor
    {
        public AnsiBackgroundColor(Color color) : base(color, AnsiColorPlane.Background)
        {
        }
        public static AnsiBackgroundColor Empty => new AnsiBackgroundColor(Color.Empty);
    }
    public class AnsiColor
    {
        private const string _ansiEscapeCode = "\u001b[";
        private const string _ansiTermination = "m";
        private const string _ansiReset = _ansiEscapeCode + "0" + _ansiTermination;

        public AnsiColor(Color color, AnsiColorPlane colorPlane)
        {
            Color = color;
            ColorPlane = colorPlane;
            AnsiCode = GetColorString();
        }

        public string AnsiCode { get; }
        public static string ResetString { get; } = _ansiReset;

        private string GetColorString()
        {
            if (Color == Color.Empty)
                return string.Empty;

            var ansi = new StringBuilder();
            ansi.Append(_ansiEscapeCode);
            var colorType = ColorPlane == AnsiColorPlane.Foreground ? "38" : "48";
            ansi.Append($"{colorType};2;");//2 needs to be there
            ansi.Append($"{Color.R};{Color.G};{Color.B}");
            ansi.Append(_ansiTermination);
            return ansi.ToString();
        }

        public Color Color { get; }
        public AnsiColorPlane ColorPlane { get; }

        public override string ToString()
        {
            return AnsiCode;
        }
    }
}
