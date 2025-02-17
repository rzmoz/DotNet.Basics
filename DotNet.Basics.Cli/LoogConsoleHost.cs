using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleHost(LoogConsoleOptions options) : IAsyncDisposable
    {
        public LoogConsoleOptions Options { get; } = options;

        public async Task<int> RunAsync(Func<LoogConsoleOptions, ILoog, Task<int>> loogContext)
        {
            var log = Options.Services.GetService<ILoog>()!;

            var exitCode = Options.FatalExitCode;

            try
            {
                if (Options is { ADO: false, Debug: true })
                {
                    Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                    Console.ReadLine();
                }

                exitCode = await loogContext.Invoke(Options, log);
            }
            catch (Exception e)
            {
                log.Info(" ");
                log.Fatal($"{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Error(e.ToString().ToMultiLine().Skip(1).JoinString(Environment.NewLine));
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
