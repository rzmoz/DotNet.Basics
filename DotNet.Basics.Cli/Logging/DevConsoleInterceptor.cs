using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleInterceptor(DevConsole console) : ICommandInterceptor
    {
        public void Intercept(CommandContext context, CommandSettings settings)
        {
            if (settings is CliCommandSettings cliSettings)
            {
                if (cliSettings.Verbose)
                    console.MinimumLogLevel = LogLevel.Trace;
                else if (cliSettings.Debug)
                    console.MinimumLogLevel = LogLevel.Debug;
                else
                    console.MinimumLogLevel = cliSettings.LogLevel;
            }

        }
    }
}
