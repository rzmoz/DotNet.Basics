using DotNet.Basics.Sys;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Cli.Logging;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli
{
    public class CliHost(CommandApp spectreApp, Func<Exception?, int> globalExceptionHandling, CliInfo info, ILogger log)
    {
        private static readonly string[] _attachDebuggerKeyWords = ["-p", "--pause"];

        private static readonly Style _exitCodeTextStyle = new Style(Color.White, null, Decoration.Dim);
        private Style GetExitCodeStyle(int exitCode) => new Style(exitCode == 0 ? Color.Default : Color.Red);

        public async Task<int> RunAsync(string[] args)
        {
            if (args.Any(a => _attachDebuggerKeyWords.Any(kw => a.EndsWith(kw, StringComparison.OrdinalIgnoreCase))))
                DevConsole.PauseForDebuggerAttach();

            var exitCode = int.MinValue;
            log.Debug($"Starting {info.ApplicationName.Highlight()} {"v".Highlight()}{info.ApplicationVersion.Highlight()}");
            var startTime = DateTime.Now;
            Exception? globalException = null;
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                //https://spectreconsole.net/cli/how-to/async-commands-and-cancellation
                Console.CancelKeyPress += (_, e) =>
                {
                    if (!cancellationTokenSource.IsCancellationRequested) //first CTRL+C requests cancellation gracefully
                    {
                        e.Cancel = true;
                        cancellationTokenSource.Cancel();
                        Console.WriteLine("\r\nCancellation requested. Press CTRL+C again to force quit.");
                    }
                };

                exitCode = await spectreApp.RunAsync(args, cancellationTokenSource.Token);

            }
            catch (Exception e)
            {
                globalException = e;
            }
            log.Debug($"{info.ApplicationName.Highlight()} finished in {(DateTime.Now - startTime).ToReadable().Highlight()}");
            if (globalException != null)
                exitCode = globalExceptionHandling(globalException);
            log.ForceWrite(("Exit code: ", _exitCodeTextStyle), (exitCode.ToString(), GetExitCodeStyle(exitCode)));
            return exitCode;
        }
    }
}