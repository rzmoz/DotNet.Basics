using System.Threading.Tasks;
using DotNet.Basics.Cli;
using DotNet.Basics.Cli.ConsoleOutput;
using DotNet.Basics.IO;
using DotNet.Basics.PowerShell;

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
                var script = "EXIT 200";
                var result = PowerShellCli.RunScript(script, log);

                return Task.CompletedTask;
            }).ConfigureAwait(false);
        }
    }
}
