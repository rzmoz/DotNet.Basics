using Spectre.Console;

namespace DotNet.Basics.Cli.Logging
{
    public class ConsoleStyleSet
    {
        public Style Default { get; init; } = Style.Plain;
        public required Style Trace { get; init; }
        public required Style Debug { get; init; }
        public required Style Info { get; init; }
        public required Style Success { get; init; }
        public required Style Warning { get; init; }
        public required Style Error { get; init; }
        public required Style Critical { get; init; }
    }
}
