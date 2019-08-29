using System;
using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.Sys;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cliHost = new CliHostBuilder(args)
                .Build();
            
            return await cliHost.RunAsync("MyTask", async (config, log) =>
            {
                log.Timing("MyMetric","finished", 10.Minutes());
                throw new CliException("swefwsfsdfsd", LogOptions.IncludeStackTrace);
            }, 1.Seconds()).ConfigureAwait(false);
        }
    }
}
