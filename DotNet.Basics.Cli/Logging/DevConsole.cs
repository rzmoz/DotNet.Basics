using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsole
    {
        private readonly AnsiConsoleLogger _logger = new();
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public static async Task StatusAsync(string status, Func<StatusContext, Task> func)
        {
            await AnsiConsole.Status().StartAsync(status, func);
        }
        public static async Task AnsiAsync(Func<IAnsiConsole, Task> func)
        {
            await func(AnsiConsole.Console);
        }
        public static void Ansi(Action<IAnsiConsole> action)
        {
            action(AnsiConsole.Console);
        }
        public static void Write(Renderable renderable)
        {
            AnsiConsole.Write(renderable);
        }

        public void Log(LogLevel level, string message, Exception? e)
        {
            if (level < MinimumLogLevel)
                return;
            _logger.Log(level, message, e);
        }
    }
}
