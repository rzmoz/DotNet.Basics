using System;
using System.Threading.Tasks;
using DotNet.Basics.Cli;
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
                .Build();

            return await cliHost.RunAsync("MyTask", async (config, log) =>
            {
                var content = @"C:\Projects\hs-sc9\src\Project\NAV\code\appsettings.json".ToFile().ReadAllText();
                
                log.Information(content);
            }).ConfigureAwait(false);
        }
    }
}
