using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    public static class DevConsoleExensions
    {
        public static async Task StatusAsync(this ILogger log, string status, Func<StatusContext, Task> func) => await DevConsole.StatusAsync(status, func);
        public static async Task ProgressAsync(this ILogger log, Func<ProgressContext, Task> func) => await DevConsole.ProgressAsync(func);
        public static void Write(this ILogger log, IRenderable renderable) => DevConsole.Write(renderable);
        public static void Ansi(this ILogger log, Action<IAnsiConsole> action) => DevConsole.Ansi(action);
        public static async Task AnsiAsync(this ILogger log, Func<IAnsiConsole, Task> func) => await DevConsole.AnsiAsync(func);
    }
}
