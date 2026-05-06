using DotNet.Basics.Sys;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using DotNet.Basics.Cli.Logging;

namespace DotNet.Basics.Cli.Console
{
    public class TestCommand(ILogger log, Greeter greeter) : CliCommand<TestCommandSettings>
    {
        protected override async Task<int> ExecuteAsync(CommandContext context, TestCommandSettings settings, CancellationToken ct)
        {
            Enum.GetValues<LogLevel>().ForEach(lvl => log.L0G(lvl, greeter.Greet(settings.Greetee)));//wo highlight
            Enum.GetValues<LogLevel>().ForEach(lvl => log.L0G(lvl, $"{lvl.ToName()} {lvl.ToName().Highlight()} lalalalalalala")); //w highlight

            log.ForceWrite(new BarChart()
                .AddItem("Apple", 12, Color.Green)
                .AddItem("Orange", 8, Color.Orange1)
                .AddItem("Banana", 5, Color.Yellow));

            var max = 99;
            await log.ProgressAsync("Doing some work...", max, async task =>
            {
                var value = 0;
                while (value < max)
                {
                    task.Value = value++;
                    await Task.Delay(5);
                }
            });

            await log.StatusAsync("Doing some more work...", async ctx =>
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
