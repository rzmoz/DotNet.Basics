using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleInterceptor(IConsoleLogger log) : ICommandInterceptor
    {
        public void Intercept(CommandContext context, CommandSettings settings)
        {            
            if (settings is CliCommandSettings cliSettings)
            {
                if (cliSettings.Verbose)
                    log.MinimumLogLevel = LogLevel.Trace;
                else if (cliSettings.Debug)
                    log.MinimumLogLevel = LogLevel.Debug;
                else
                    log.MinimumLogLevel = cliSettings.LogLevel;
            }
        }
    }
}
