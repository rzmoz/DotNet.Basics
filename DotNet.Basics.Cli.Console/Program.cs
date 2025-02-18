using DotNet.Basics.Pipelines;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            await using var host = new LoogConsoleBuilder(args)
                .Services(services =>
                {
                    services.AddPipelines();
                })
                .Configure(o => o.WithExceptionHandler<PipelineException>(e => e.ExitCode))
                .Build();

            return await host.RunAsync(async (lops, log) =>
            {
                return await lops.StartAsync("logging", () =>
                {
                    log.Verbose($"{nameof(log.Verbose)} {nameof(log.Verbose).Highlight()} lalalalalalala");
                    log.Debug($"{nameof(log.Debug)} {nameof(log.Debug).Highlight()} lalalalalalala");
                    log.Info($"{nameof(log.Info)} {nameof(log.Info).Highlight()} lalalalalalala");
                    log.Success($"{nameof(log.Success)} {nameof(log.Success).Highlight()} lalalalalalala");
                    log.Warning($"{nameof(log.Warning)} {nameof(log.Warning).Highlight()} lalalalalalala");
                    log.Error($"{nameof(log.Error)} {nameof(log.Error).Highlight()} lalalalalalala");
                    log.Fatal($"{nameof(log.Fatal)} {nameof(log.Fatal).Highlight()} lalalalalalala");
                    return Task.FromResult(-6);
                });
            });
        }
    }
}
