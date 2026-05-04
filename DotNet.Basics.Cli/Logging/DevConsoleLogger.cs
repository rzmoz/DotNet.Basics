using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsoleLogger : ILogger
    {
        public LogLevel MinimumLogLevel { get => _logger.MinimumLogLevel; set => _logger.MinimumLogLevel = value; }

        private readonly AnsiConsoleLogger _logger = new();

        public void Log(LogLevel level, string message, Exception? e) => _logger.Log(level, message, e);

        public static void PauseForDebuggerAttach() => AnsiConsole.Prompt(new TextPrompt<string>($"Pausing to attach debugger <{Environment.ProcessId}>. [yellow]Press enter to continue[/]").AllowEmpty());

        public async Task StatusAsync(LogLevel lvl, string status, Func<StatusContext, Task> func) => await RunIfLogLevelAsync(lvl, async () => await AnsiConsole.Status().StartAsync(status, func));
        public async Task ProgressAsync(LogLevel lvl, Func<ProgressContext, Task> func) => await RunIfLogLevelAsync(lvl, async () => await AnsiConsole.Progress().StartAsync(func));
        public void Write(LogLevel lvl, IRenderable renderable) => RunIfLogLevel(lvl, () => AnsiConsole.Write(renderable));
        public void Ansi(LogLevel lvl, Action<IAnsiConsole> action) => RunIfLogLevel(lvl, () => action(AnsiConsole.Console));
        public async Task AnsiAsync(LogLevel lvl, Func<IAnsiConsole, Task> func) => await RunIfLogLevelAsync(lvl, async () => await func(AnsiConsole.Console));

        private void RunIfLogLevel(LogLevel lvl, Action action)
        {
            if (IsEnabled(lvl))
                action();
        }
        private async Task RunIfLogLevelAsync(LogLevel lvl, Func<Task> func)
        {
            if (IsEnabled(lvl))
                await func();
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= MinimumLogLevel;

        public void Log<TState>(LogLevel lvl, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logger.Log(lvl, formatter.Invoke(state, exception), exception);
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    }
}
