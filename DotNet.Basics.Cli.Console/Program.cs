using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = new CliHostBuilder()
                .WithServices(s => s.AddSingleton<Greeter>())
                .Build<TestCommand, TestCommandSettings>(isDefault: false);

            return await host.RunAsync(args);
        }
    }
}
