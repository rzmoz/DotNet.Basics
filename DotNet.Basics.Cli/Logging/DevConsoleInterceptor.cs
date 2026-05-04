using DotNet.Basics.Diagnostics;
using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleInterceptor(EventLogger eventLogger) : ICommandInterceptor
    {
        private DevConsole _console = new DevConsole();

        public void Intercept(CommandContext context, CommandSettings settings)
        {
            eventLogger.MessageLogged += _console.Log;
            _console.MinimumLogLevel = (settings as CliCommandSettings)?.LogLevel ?? _console.MinimumLogLevel;
        }
    }
}
