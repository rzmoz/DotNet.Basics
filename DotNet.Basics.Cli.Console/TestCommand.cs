using DotNet.Basics.Sys;
using DotNet.Basics.Cli.Logging;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Console
{
    public class TestCommand(DevConsoleLogger log, Greeter greeter) : CliCommand<TestCommandSettings>
    {
        protected override async Task<int> ExecuteAsync(CommandContext context, TestCommandSettings settings, CancellationToken cancellationToken)
        {
            Enum.GetValues<LogLevel>().ForEach(lvl => log.L0G(lvl, greeter.Greet(settings.Greetee)));//wo highlight
            Enum.GetValues<LogLevel>().ForEach(lvl => log.L0G(lvl, $"{lvl.ToName()}{lvl.ToName().Highlight()} lalalalalalala")); //w highlight

            log.Write(LogLevel.Information, new BarChart()
                .AddItem("Apple", 12, Color.Green)
                .AddItem("Orange", 8, Color.Orange1)
                .AddItem("Banana", 5, Color.Yellow));


            await log.StatusAsync(LogLevel.Information, "Doing some work...", async ctx =>
            {
                await Task.Delay(500);
                ctx.Status("Still working...");
                await Task.Delay(500);
                ctx.Status("Almost done...");
                await Task.Delay(500);
            });
            throw new IOException("This is a test exception");
        }
    }
}
