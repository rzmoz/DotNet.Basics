using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotNet.Basics.Serilog.Cli
{
    public class ConsoleHostContext(bool verbose = false, bool ado = false, bool debug = false, Action<IServiceCollection>? configureServices = null) : IAsyncDisposable
    {
        public static readonly string DebugFlag = "--debug";
        public static readonly string VerboseFlag = "--verbose";
        public static readonly string ADOFlag = "--ADO";

        public ConsoleHostContext(string[] args, Action<IServiceCollection>? configureServices = null)
        : this(verbose: HasFlag(VerboseFlag, args), ado: HasFlag(ADOFlag, args), debug: HasFlag(DebugFlag, args), configureServices)
        { }

        public int FatalExitCode { get; set; } = 500;
        public IDictionary<string, Func<Exception, int>> ExceptionExitCodes { get; set; } = new Dictionary<string, Func<Exception, int>>();

        public async Task<int> RunAsync(Func<IServiceProvider, ILoog, Task<int>> loogContext)
        {
            var serviceCollection = new ServiceCollection();
            configureServices?.Invoke(serviceCollection);
            serviceCollection.AddDiagnosticsWithSerilogDevConsole(verbose: verbose, ado: ado);
            var services = serviceCollection.BuildServiceProvider();
            var log = services.GetService<ILoog>()!;
            try
            {
                if (!ado && debug)
                {
                    Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                    Console.ReadLine();
                }

                return await loogContext.Invoke(services, log);
            }
            catch (Exception e)
            {
                log.Info(" ");
                log.Fatal($"{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Error(e.ToString().ToMultiLine().Skip(1).JoinString(Environment.NewLine));
                return ExceptionExitCodes.TryGetValue(e.GetType().Name, out var resolver) ? resolver.Invoke(e) : FatalExitCode;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Log.CloseAndFlushAsync();
        }

        private static bool HasFlag(string flag, string[] args)
        {
            return args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
