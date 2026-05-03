using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    public static class DevConsoleExensions
    {
        public static void Write(this ILogger log, Renderable renderable)
        {
            AnsiConsole.Write(renderable);
        }
        public static void Ansi(this ILogger log, Action<IAnsiConsole> action)
        {
            action(AnsiConsole.Console);
        }
        public static async Task AnsiAsync(this ILogger log, Func<IAnsiConsole, Task> func)
        {
            await func(AnsiConsole.Console);
        }
        public static async Task StatusAsync(this ILogger log, string status, Func<StatusContext, Task> func)
        {
            await AnsiConsole.Status().StartAsync(status, func);
        }


    }
}
