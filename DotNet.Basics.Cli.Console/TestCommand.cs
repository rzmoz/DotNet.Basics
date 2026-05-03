using DotNet.Basics.Cli.Logging;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DotNet.Basics.Cli.Console
{
    public class TestCommand(ILogger log) : CliCommand
    {
        protected override async Task<int> ExecuteAsync(CommandContext context, CliCommandSettings settings, CancellationToken cancellationToken)
        {
            log.Trace($"Trace {"Trace".Highlight()} lalalalalalala");
            log.Debug($"Debug {"Debug".Highlight()} lalalalalalala");
            log.Info($"Info {"Info".Highlight()} lalalalalalala");
            log.Success($"Success {"Success".Highlight()} lalalalalalala");
            log.Warn($"Warn {"Warn".Highlight()} lalalalalalala");
            log.Error($"Error {"Error".Highlight()} lalalalalalala");
            log.Critical($"Critical {"Critical".Highlight()} lalalalalalala");
            log.Write(new BarChart()
                .AddItem("Apple", 12, Color.Green)
                .AddItem("Orange", 8, Color.Orange1)
                .AddItem("Banana", 5, Color.Yellow));
            await log.StatusAsync("Doing some work...", async ctx =>
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
