using DotNet.Basics.Cli.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli
{
    public class CliHost(CommandApp spectreApp, Func<Exception, int> globalExceptionHandling)
    {
        public async Task<int> RunAsync(string[] args)
        {
            if (args.Any(a => a.EndsWith("-debug", StringComparison.OrdinalIgnoreCase)))
                DevConsole.PauseForDebuggerAttach();

            var exitCode = int.MinValue;

            try
            {
                exitCode = await spectreApp.RunAsync(args);
            }
            catch (Exception e)
            {
                exitCode = globalExceptionHandling(e);
            }
            finally
            {
                DevConsole.Ansi(console =>
                {
                    console.Write(new Text("Exit code:", new Style(Color.White, null, Decoration.Dim)));
                    console.Write(" ");
                    console.Write(new Text(exitCode.ToString(), new Style(exitCode == 0 ? Color.Default : Color.Red)));
                });
            }
            return exitCode;
        }
    }
}