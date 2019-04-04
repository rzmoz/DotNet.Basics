using System.Drawing;
using System.Text;

namespace DotNet.Basics.Cli
{
    public class AnsiForegroundColor : AnsiColor
    {
        public AnsiForegroundColor(Color color, bool bold = false, bool underline = false) : base(color, AnsiColorPlane.Foreground, bold, underline)
        {
        }
        public static AnsiForegroundColor Empty => new AnsiForegroundColor(Color.Empty);
    }
    public class AnsiBackgroundColor : AnsiColor
    {
        public AnsiBackgroundColor(Color color, bool bold = false, bool underline = false) : base(color, AnsiColorPlane.Background, bold, underline)
        {
        }
        public static AnsiBackgroundColor Empty => new AnsiBackgroundColor(Color.Empty);
    }
    public class AnsiColor
    {
        public AnsiColor(Color color, AnsiColorPlane colorPlane, bool bold = false, bool underline = false)
        {
            Color = color;
            ColorPlane = colorPlane;
            Bold = bold;
            Underline = underline;
            ColorString = GetColorString();
        }

        public string ColorString { get; }

        private string GetColorString()
        {
            if (Color == Color.Empty)
                return string.Empty;

            var ansi = new StringBuilder();
            var colorType = ColorPlane == AnsiColorPlane.Foreground ? "38" : "48";
            ansi.Append($"{colorType};");
            ansi.Append(Bold ? "1;" : "2;");
            if (Underline)
                ansi.Append("4;");
            ansi.Append($"{Color.R};{Color.G};{Color.B}");
            return ansi.ToString();
        }

        public Color Color { get; }
        public AnsiColorPlane ColorPlane { get; }
        public bool Bold { get; }
        public bool Underline { get; }

        public override string ToString()
        {
            return ColorString;
        }
    }
}
