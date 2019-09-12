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
                var script = @"C:\Projects\hs-sc9\scripts\Solution.PostBuild.Callback.ps1 -slnDir C:\Projects\hs-sc9\scripts -artifactsDir C:\Projects\hs-sc9\.releaseArtifacts";
                var result = PowerShellCli.RunScript(script, log);

                return Task.FromResult(200);
            }).ConfigureAwait(false);
        }
    }
}
