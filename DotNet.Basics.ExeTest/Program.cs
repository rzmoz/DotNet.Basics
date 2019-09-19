using System;
using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.Cli.ConsoleOutput;
using DotNet.Basics.Diagnostics;

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

            return await cliHost.RunAsync("MyTask", (config, log) =>
            {
                foreach (LogLevel level in (LogLevel[])Enum.GetValues(typeof(LogLevel)))
                {
                    cliHost.Log.Write(level, $"{level}: Hello {"World!".Highlight()}");
                    cliHost.Log.Write(level, "");
                }
                return Task.FromResult(0);
            }).ConfigureAwait(false);
        }
    }
}

