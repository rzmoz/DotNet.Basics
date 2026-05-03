using Spectre.Console.Cli;

namespace DotNet.Basics.Cli
{
    public abstract class CliCommand<TSettings> : AsyncCommand<TSettings> where TSettings : CliCommandSettings
    { }
    public abstract class CliCommand : CliCommand<CliCommandSettings>
    { }
}
