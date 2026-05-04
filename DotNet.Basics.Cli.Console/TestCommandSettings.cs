using Spectre.Console.Cli;
using System.ComponentModel;

namespace DotNet.Basics.Cli.Console
{
    public class TestCommandSettings : CliCommandSettings
    {
        [CommandArgument(0, $"[{nameof(Greetee)}]")]
        [Description("The operation to perform")]
        [DefaultValue("World")]
        public string Greetee { get; init; } = "World";
    }
}
