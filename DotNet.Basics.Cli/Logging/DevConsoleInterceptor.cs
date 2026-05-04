using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleInterceptor(DevConsole console) : ICommandInterceptor
    {
        public void Intercept(CommandContext context, CommandSettings settings)
        {
            if (settings is CliCommandSettings cliSettings)            
                console.MinimumLogLevel = cliSettings.Verbose ? LogLevel.Trace : cliSettings.LogLevel;
        }
    }
}
