using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsole : ILogger
    {
        public static readonly DevConsole Console = new DevConsole { MinimumLogLevel = LogLevel.Trace };
        public static readonly IAnsiConsole AnsiConsole = Spectre.Console.AnsiConsole.Console;
        public static void PauseForDebuggerAttach() => AnsiConsole.Prompt(new TextPrompt<string>($"Pausing to attach debugger <{Environment.ProcessId}>. [yellow]Press enter to continue[/]").AllowEmpty());

        private readonly AnsiConsoleLogger _logger = new();

        public LogLevel MinimumLogLevel { get => _logger.MinimumLogLevel; set => _logger.MinimumLogLevel = value; }

        public bool IsEnabled(LogLevel lvl) => lvl >= MinimumLogLevel && lvl < LogLevel.None;
        public void Log(LogLevel lvl, string msg, Exception? e) => _logger.Log(lvl, msg, e);
        public void Log<TState>(LogLevel lvl, EventId eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter) => _logger.Log(lvl, formatter.Invoke(state, e), e);
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public async Task StatusAsync(string status, Func<StatusContext, Task> func) => await AnsiConsole.Status().StartAsync(status, func);
        public async Task ProgressAsync(Func<ProgressContext, Task> func) => await AnsiConsole.Progress().StartAsync(func);
        public void Write(IRenderable renderable) => AnsiConsole.Write(renderable);
        public void Write(string str) => AnsiConsole.Write(str);
        public void Ansi(Action<IAnsiConsole> action) => action(AnsiConsole);
        public async Task AnsiAsync(Func<IAnsiConsole, Task> func) => await func(AnsiConsole);
    }
}
