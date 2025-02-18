using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Basics.Serilog.Looging;
using Serilog;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleHost(LoogConsoleOptions options) : IAsyncDisposable
    {
        private const string _newlinePattern = @"\r\n|\r|\n";
        private static readonly Regex _newlineRegex = new(_newlinePattern, RegexOptions.Compiled);
        public LoogConsoleOptions Options { get; } = options;

        public async Task<int> RunAsync(Func<Task<int>> loogContext)
        {
            return await RunAsync(async (_, _, _) => await loogContext());
        }
        public async Task<int> RunAsync(Func<ILoog, Task<int>> loogContext)
        {
            return await RunAsync(async (_, _, log) => await loogContext(log));
        }
        public async Task<int> RunAsync(Func<LongRunningOperations, ILoog, Task<int>> loogContext)
        {
            return await RunAsync(async (_, ops, log) => await loogContext(ops, log));
        }
        public async Task<int> RunAsync(Func<LoogConsoleOptions, LongRunningOperations, ILoog, Task<int>> loogContext)
        {
            var log = Options.GetService<ILoog>()!;
            var longRunningOperations = options.GetService<LongRunningOperations>();
            var exitCode = Options.FatalExitCode;

            try
            {
                if (Options is { ADO: false, Debug: true })
                {
                    Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                    Console.ReadLine();
                }

                exitCode = await loogContext.Invoke(Options, longRunningOperations, log);
            }
            catch (Exception e)
            {
                log.Info(" ");
                log.Fatal($"{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Error(string.Join(Environment.NewLine, _newlineRegex.Split(e.ToString()).Skip(1)));
                exitCode = Options.ExceptionHandlers.TryGetValue(e.GetType().Name, out var resolver) ? resolver.Invoke(e) : Options.FatalExitCode;
            }
            finally
            {
                log.Verbose($"Exit code: {exitCode.ToString().Highlight()}");
                Console.ResetColor();
            }
            return exitCode;
        }

        public async ValueTask DisposeAsync()
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
