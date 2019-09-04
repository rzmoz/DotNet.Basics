using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.PowerShell;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cliHost = new CliHostBuilder(args, mappings => mappings.Add("lorem", "ipsum"))
                .Build();

            return await cliHost.RunAsync("MyTask", async (config, log) =>
                {
                    PowerShellCli.Run(log, @"Write-Host ""Hello World!""");
                }).ConfigureAwait(false);
        }
    }
}
