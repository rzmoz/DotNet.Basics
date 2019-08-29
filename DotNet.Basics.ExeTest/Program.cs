using System;
using System.Threading.Tasks;
using DotNet.Basics.Cli;

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

                log.Metric("MyMetric", 2.5333);

            }).ConfigureAwait(false);
        }
    }
}
