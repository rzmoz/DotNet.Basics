using DotNet.Basics.Serilog.Cli;
using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Serilog.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            await using var ctx = new ConsoleLoogContext(args, 1.Seconds())
                .WithExceptionHandler<ExitCodeException>(e => e.ExitCode);

            return await ctx.RunAsync(async (services, log) =>
            {
                var longOps = (LongRunningOperations)services.GetService(typeof(LongRunningOperations))!;
                return await longOps.StartAsync("logging", async () =>
                {
                    /*log.Verbose($"{nameof(log.Verbose)} {nameof(log.Verbose).Highlight()} lalalalalalala");
                    log.Debug($"{nameof(log.Debug)} {nameof(log.Debug).Highlight()} lalalalalalala");
                    log.Info($"{nameof(log.Info)} {nameof(log.Info).Highlight()} lalalalalalala");
                    log.Success($"{nameof(log.Success)} {nameof(log.Success).Highlight()} lalalalalalala");
                    log.Warning($"{nameof(log.Warning)} {nameof(log.Warning).Highlight()} lalalalalalala");
                    log.Error($"{nameof(log.Error)} {nameof(log.Error).Highlight()} lalalalalalala");
                    log.Fatal($"{nameof(log.Fatal)} {nameof(log.Fatal).Highlight()} lalalalalalala");*/
                    
                    return 005;
                });
            });
        }
    }
}
