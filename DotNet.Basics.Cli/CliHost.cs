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
    public class CliHost(CommandApp spectreApp, Func<Exception?, int> globalExceptionHandling, CliInfo info)
    {
        private static readonly string[] _attachDebuggerKeyWords = ["-d", "-debug"];

        private static readonly Style _exitCodeTextStyle = new Style(Color.White, null, Decoration.Dim);
        private Style GetExitCodeStyle(int exitCode) => new Style(exitCode == 0 ? Color.Default : Color.Red);

        public async Task<int> RunAsync(string[] args)
        {
            return await RunAsync(args, 3.Seconds());
        }

        /// <param name="args">cli args</param>
        /// <param name="forceQuitTimeout">First CTRL+C triggers cancellation token request - second CTRL+C forces quits</param>
        /// <returns></returns>
        public async Task<int> RunAsync(string[] args, TimeSpan forceQuitTimeout)
        {
            if (args.Any(a => _attachDebuggerKeyWords.Any(kw => a.EndsWith(kw, StringComparison.OrdinalIgnoreCase))))
                DevConsole.PauseForDebuggerAttach();

            var exitCode = int.MinValue;
            DevConsole.Console.Log(LogLevel.Debug, $"Starting {info.ApplicationName.Highlight()} {"v".Highlight()}{info.ApplicationVersion.Highlight()}");
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
            DevConsole.Console.Log(LogLevel.Debug, $"{info.ApplicationName.Highlight()} finished in {(DateTime.Now - startTime).ToReadable().Highlight()}");
            if (globalException != null)
                exitCode = globalExceptionHandling(globalException);
            DevConsole.Console.ForceWrite(("Exit code: ", _exitCodeTextStyle), (exitCode.ToString(), GetExitCodeStyle(exitCode)));
            return exitCode;
        }
    }
}