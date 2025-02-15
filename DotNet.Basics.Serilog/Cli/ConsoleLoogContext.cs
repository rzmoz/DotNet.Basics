using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotNet.Basics.Serilog.Cli
{
    public class ConsoleLoogContext(bool verbose, bool ado, bool debug, TimeSpan longRunningOperationsPingInterval, Action<IServiceCollection>? configureServices = null) : IAsyncDisposable
    {
        public static readonly string DebugFlag = "--debug";
        public static readonly string VerboseFlag = "--verbose";
        public static readonly string ADOFlag = "--ADO";

        private readonly Dictionary<string, Func<Exception, int>> _exceptionHandlers = new();

        public ConsoleLoogContext(string[] args, Action<IServiceCollection>? configureServices = null)
            : this(args, 1.Minutes(), configureServices)
        { }
        public ConsoleLoogContext(string[] args, TimeSpan longRunningOperationsPingInterval, Action<IServiceCollection>? configureServices = null)
        : this(verbose: HasFlag(VerboseFlag, args), ado: HasFlag(ADOFlag, args), debug: HasFlag(DebugFlag, args), longRunningOperationsPingInterval, configureServices)
        { }

        public int FatalExitCode { get; set; } = 500;

        public ConsoleLoogContext WithExceptionHandler<T>(Func<T, int> exceptionHandler) where T : Exception
        {
            _exceptionHandlers.Add(typeof(T).Name, e => exceptionHandler.Invoke((T)e));
            return this;
        }

        public async Task<int> RunAsync(Func<IServiceProvider, ILoog, Task<int>> loogContext)
        {
            var serviceCollection = new ServiceCollection();
            configureServices?.Invoke(serviceCollection);
            serviceCollection.AddDiagnosticsWithSerilogDevConsole(verbose: verbose, ado: ado, longRunningOperationsPingInterval: longRunningOperationsPingInterval);
            var services = serviceCollection.BuildServiceProvider();
            var log = services.GetService<ILoog>()!;

            var exitCode = FatalExitCode;

            try
            {
                if (!ado && debug)
                {
                    Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                    Console.ReadLine();
                }

                exitCode = await loogContext.Invoke(services, log);
            }
            catch (Exception e)
            {
                log.Info(" ");
                log.Fatal($"{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Error(e.ToString().ToMultiLine().Skip(1).JoinString(Environment.NewLine));
                exitCode = _exceptionHandlers.TryGetValue(e.GetType().Name, out var resolver) ? resolver.Invoke(e) : FatalExitCode;
            }
            finally
            {
                log.Debug($"Exit code: {exitCode.ToString().Highlight()}");
                Console.ResetColor();
            }
            return exitCode;
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
