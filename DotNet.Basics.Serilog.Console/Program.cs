using DotNet.Basics.Serilog.Cli;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Serilog.Console
{
    internal class Program
    {
        private const string _verboseFlag = "--verbose";
        private const string _isADOFlag = "--ado";

        static async Task<int> Main(string[] args)
        {
            var verbose = args.Any(a => a.Equals(_verboseFlag, StringComparison.OrdinalIgnoreCase));
            var isADO = args.Any(a => a.Equals(_isADOFlag, StringComparison.OrdinalIgnoreCase));

            await using var ctx = new ConsoleHostContext(verbose: verbose, ado: isADO)
            {
                ExceptionExitCodes =
                {
                    {nameof(ExceptionHandlingException), e => 400}
                }
            };
            return await ctx.RunAsync(async (services, log) =>
            {
                log.Verbose($"{nameof(log.Verbose)} {nameof(log.Verbose).Highlight()} lalalalalalala");
                log.Debug($"{nameof(log.Debug)} {nameof(log.Debug).Highlight()} lalalalalalala");
                log.Info($"{nameof(log.Info)} {nameof(log.Info).Highlight()} lalalalalalala");
                log.Success($"{nameof(log.Success)} {nameof(log.Success).Highlight()} lalalalalalala");
                log.Warning($"{nameof(log.Warning)} {nameof(log.Warning).Highlight()} lalalalalalala");
                log.Error($"{nameof(log.Error)} {nameof(log.Error).Highlight()} lalalalalalala");
                log.Fatal($"{nameof(log.Fatal)} {nameof(log.Fatal).Highlight()} lalalalalalala");
                throw new IOException("Halleluja");
            });
        }
    }
}
