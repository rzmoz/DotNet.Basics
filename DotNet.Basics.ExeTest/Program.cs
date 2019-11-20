using System;
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
                .WithLogging(config => config.AddFirstSupportedConsole())
                .Build(mappings => mappings.Add("lorem", "ipsum"));

            LongRunningOperations.Init(1.Seconds());

            return await cliHost.RunAsync("MyTask", (config, log) =>
            {
                log.Raw("Hello World!");

                return Task.FromResult(0);
            }).ConfigureAwait(false);
        }
    }
}

