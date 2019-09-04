using System.Threading.Tasks;
using DotNet.Basics.Cli;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cliHost = new CliHostBuilder(args, mappings => mappings.Add("lorem","ipsum"))
                .Build();
            
            return await cliHost.RunAsync("MyTask", async (config, log) =>
            {
                log.Warning(config["ipsum"]);

            }).ConfigureAwait(false);
        }
    }
}
