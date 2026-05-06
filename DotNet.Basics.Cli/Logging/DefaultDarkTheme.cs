using Spectre.Console;

namespace DotNet.Basics.Cli.Logging
{
    public class DefaultDarkTheme : ConsoleStyleTheme
    {
        public DefaultDarkTheme() : base(Primary, Highlights) { }

        public static readonly ConsoleStyleSet Primary = new()
        {
            Trace = new Style(Color.Gray, null, Decoration.Dim),
            Debug = new Style(Color.Gray),
            Info = new Style(Color.DarkCyan),
            Success = new Style(Color.Green),
            Warning = new Style(Color.Yellow),
            Error = new Style(Color.Red),
            Critical = new Style(Color.White, Color.DarkRed)
        };

        public static readonly ConsoleStyleSet Highlights = new()
        {
            Trace = new Style(Color.Gray),
            Debug = new Style(Color.LightSlateGrey),
            Info = new Style(Color.Cyan),
            Success = new Style(Color.GreenYellow),
            Warning = new Style(Color.Orange1),
            Error = new Style(Color.White),
            Critical = new Style(Color.White, Color.DarkRed)
        };
    }
}
