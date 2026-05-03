namespace DotNet.Basics.Cli.Console
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = new CliHostBuilder()
                .Build<TestCommand>();

            return await host.RunAsync(args);
        }
    }
}
