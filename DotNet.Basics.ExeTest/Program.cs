using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.Diagnostics.Console;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            args.PauseIfDebug();
            var cliHost = new CliHostBuilder(args)
                .WithLogging(config => config.AddFirstSupportedConsole())
                .Build<ExeTestArgs>(mappings => mappings.Add("val", "value"));

            return await cliHost.RunAsync("MyTask", (config, log) =>
            {
                log.Raw($"{nameof(ExeTestArgs.Value)}: {cliHost.Args.Value}");

                return Task.FromResult(0);
            }).ConfigureAwait(false);
        }
    }
}

