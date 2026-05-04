using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await new CliHostBuilder()
                .WithServices(s => s.AddSingleton<Greeter>())
                .WithCommand<TestCommand>(false)
                .Build()
                .RunAsync(args);
        }
    }
}
