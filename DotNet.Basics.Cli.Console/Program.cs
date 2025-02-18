using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            await using var host = new LoogConsoleBuilder(args)
                .Services(services =>
                {
                    services.AddPipelines();
                })
                .Build();


            return await host.RunPipelineAsync<TestPipeline>(host.Options);
        }
    }
}
