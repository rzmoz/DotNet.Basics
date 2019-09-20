using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.Cli.ConsoleOutput;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            args.PauseIfDebug();
            var cliHost = new CliHostBuilder(args, mappings => mappings.Add("lorem", "ipsum"))
                .WithLogging(config => config.AddFirstSupportedConsole())
                .Build();

            LongRunningOperations.Init(1.Seconds());

            return await cliHost.RunAsync("MyTask", async (config, log) =>
            {
                var levels = (IEnumerable<LogLevel>)Enum.GetValues(typeof(LogLevel));
                await levels.ForEachParallelAsync(async level =>
                {
                    await LongRunningOperations.StartAsync(level.ToName(), async () =>
                    {
                        cliHost.Log.Write(level, $"{level}: Hello {"World!".Highlight()}");
                        cliHost.Log.Write(level, string.Empty);

                        throw new ApplicationException(level.ToName());
                        return 0;

                    }).ConfigureAwait(false);
                }).ConfigureAwait(false);
                return 0;
            }).ConfigureAwait(false);
        }
    }
}

