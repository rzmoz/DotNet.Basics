using System.Threading.Tasks;
using DotNet.Basics.Cli;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            args.PauseIfDebug();

            var builder = Host.CreateApplicationBuilder().BuildManaged((_, _) =>
            {
                Log.Logger.Information("Running in managed Builder");
            });

            return await builder.Build().RunManagedAsync(host =>
            {
                Log.Logger.Information("Running in managed RunAsync");
                return host.RunAsync();
            });
        }
    }
}

