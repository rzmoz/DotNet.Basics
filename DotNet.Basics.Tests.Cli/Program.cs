using DotNet.Basics.Cli;
using Serilog.Events;

namespace DotNet.Basics.Tests.Cli
{
    internal class Program
    {
        static int Main(string[] args)
        {
            return new CliHost(args)
                .Run(log =>
                {
                    try
                    {
                        log.Verbose($"{nameof(log.Verbose)}: {{verbose}}", LogEventLevel.Verbose);
                        log.Debug($"{nameof(log.Debug)}: {{Debug}}", LogEventLevel.Debug);
                        log.Information($"{nameof(log.Information)}: {{Information}}", LogEventLevel.Information);
                        log.Warning($"{nameof(log.Warning)}: {{Warning}}", LogEventLevel.Warning);
                        log.Error($"{nameof(log.Error)}: {{Error}}", LogEventLevel.Error);
                        log.Fatal($"{nameof(log.Fatal)}: {{Fatal}}", LogEventLevel.Fatal);
                        return int.Parse(args[0]);
                    }
                    catch (FormatException)
                    {
                        return -11111;
                    }

                });
        }
    }
}
