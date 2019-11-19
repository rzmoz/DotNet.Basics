using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.Cli.ConsoleOutput;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            args.PauseIfDebug();
            var cliHost = new CliHostBuilder(args)
                .WithArgsSwitchMappings(mappings => mappings.Add("lorem", "ipsum"))
                .WithLogging(config => config.AddFirstSupportedConsole())
                .Build();

            LongRunningOperations.Init(1.Seconds());

            return await cliHost.RunAsync("MyTask", async (arg, config, log) =>
            {
                log.Raw("Hello World!");

                return 0;
            }).ConfigureAwait(false);
        }
    }
}

