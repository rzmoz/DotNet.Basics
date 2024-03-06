using DotNet.Basics.Cli;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tests.NetCore.EchoOut
{
    public class EchoOutProgram
    {
        public static async Task Main(string[] args)
        {
            await new CliHost(args)
                .RunAsync(async log =>
                {
                    try
                    {
                        log.Verbose(nameof(log.Verbose));
                        log.Debug(nameof(log.Debug));
                        log.Information(nameof(log.Information));
                        log.Warning(nameof(log.Warning));
                        log.Error(nameof(log.Error));
                        log.Fatal(nameof(log.Fatal));
                        return int.Parse(args[0]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return -100000;
                    }
                }).ConfigureAwait(false);
        }
    }
}
