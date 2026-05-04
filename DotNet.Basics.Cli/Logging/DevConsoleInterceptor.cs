using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleInterceptor(DevConsoleLogger console) : ICommandInterceptor
    {
        public void Intercept(CommandContext context, CommandSettings settings)
        {
            console.MinimumLogLevel = (settings as CliCommandSettings)?.LogLevel ?? console.MinimumLogLevel;
        }
    }
}
