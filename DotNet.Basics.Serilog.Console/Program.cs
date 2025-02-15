using DotNet.Basics.Serilog.Looging;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Serilog.Console
{
    internal class Program
    {
        private const string _verboseFlag = "--verbose";
        private const string _isADOFlag = "--isADO";

        static void Main(string[] args)
        {
            var verbose = args.Any(a => a.Equals(_verboseFlag, StringComparison.OrdinalIgnoreCase));
            var isADO = args.Any(a => a.Equals(_isADOFlag, StringComparison.OrdinalIgnoreCase));

            var services = new ServiceCollection().AddDiagnosticsWithSerilogDevConsole(verbose: verbose, isADO: isADO).BuildServiceProvider();
            var log = services.GetService<ILoog>()!;

            log.Verbose($"{nameof(log.Verbose)} {nameof(log.Verbose).Highlight()} lalalalalalala");
            log.Debug($"{nameof(log.Debug)} {nameof(log.Debug).Highlight()} lalalalalalala");
            log.Info($"{nameof(log.Info)} {nameof(log.Info).Highlight()} lalalalalalala");
            log.Success($"{nameof(log.Success)} {nameof(log.Success).Highlight()} lalalalalalala");
            log.Warning($"{nameof(log.Warning)} {nameof(log.Warning).Highlight()} lalalalalalala");
            log.Error($"{nameof(log.Error)} {nameof(log.Error).Highlight()} lalalalalalala");
            log.Fatal($"{nameof(log.Fatal)} {nameof(log.Fatal).Highlight()} lalalalalalala");
        }
    }
}
