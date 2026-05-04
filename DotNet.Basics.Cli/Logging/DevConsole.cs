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

        public LogLevel MinimumLogLevel { get => _logger.MinimumLogLevel; set => _logger.MinimumLogLevel = value; }

        public void Log(LogLevel level, string message, Exception? e) => _logger.Log(level, message, e);

        public static void PauseForDebuggerAttach()=> AnsiConsole.Prompt(new TextPrompt<string>($"Pausing to attach debugger <{Environment.ProcessId}>. [yellow]Press enter to continue[/]").AllowEmpty());

        public static async Task StatusAsync(string status, Func<StatusContext, Task> func) => await AnsiConsole.Status().StartAsync(status, func);
        public static async Task ProgressAsync(Func<ProgressContext, Task> func) => await AnsiConsole.Progress().StartAsync(func);
        public static void Write(IRenderable renderable) => AnsiConsole.Write(renderable);
        public static void Ansi(Action<IAnsiConsole> action) => action(AnsiConsole.Console);
        public static async Task AnsiAsync(Func<IAnsiConsole, Task> func) => await func(AnsiConsole.Console);
    }
}
