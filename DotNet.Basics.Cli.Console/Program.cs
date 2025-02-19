using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            await using var host = new CliHostBuilder(args)
                .WithOptions()
                .WithSerilogDevConsole()
                .WithServices(services =>
                {
                    services.AddPipelines();
                    services.AddPipelineSteps();
                }).Build();

            return await host.RunPipelineAsync<TestPipeline>();
        }
    }
}
