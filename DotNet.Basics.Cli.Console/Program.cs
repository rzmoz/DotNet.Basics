using DotNet.Basics.Serilog.Looging;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            await using var host = new LoogConsoleBuilder(args)
                .Services(services =>
                {
                    services.AddTransient<ExitCodeException>();
                })
                .Configure(o => o.WithExceptionHandler<ExitCodeException>(e => e.ExitCode))
                .Build();

            return await host.RunAsync(async (services, log) =>
            {
                var longOps = services.Services.GetService<LongRunningOperations>()!;
                return await longOps.StartAsync("logging", async () =>
                {
                    log.Verbose($"{nameof(log.Verbose)} {nameof(log.Verbose).Highlight()} lalalalalalala");
                    log.Debug($"{nameof(log.Debug)} {nameof(log.Debug).Highlight()} lalalalalalala");
                    log.Info($"{nameof(log.Info)} {nameof(log.Info).Highlight()} lalalalalalala");
                    log.Success($"{nameof(log.Success)} {nameof(log.Success).Highlight()} lalalalalalala");
                    log.Warning($"{nameof(log.Warning)} {nameof(log.Warning).Highlight()} lalalalalalala");
                    log.Error($"{nameof(log.Error)} {nameof(log.Error).Highlight()} lalalalalalala");
                    log.Fatal($"{nameof(log.Fatal)} {nameof(log.Fatal).Highlight()} lalalalalalala");
                    throw new IOException();
                });
            });
        }
    }
}
