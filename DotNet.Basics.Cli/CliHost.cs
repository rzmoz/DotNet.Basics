using DotNet.Basics.Cli.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli
{
    public class CliHost(CommandApp spectreApp)
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
                AnsiConsole.WriteException(e, new ExceptionSettings
                {
                    Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks,
                    Style = new ExceptionStyle
                    {
                        Exception = new Style(Color.White, Color.DarkRed),
                        Message = new Style(Color.Red),
                        LineNumber = new Style(Color.Blue),
                    }
                });
                if (int.TryParse(e.GetType().GetProperty("ExitCode")?.GetValue(e)?.ToString(), out int result))
                    exitCode = result;
            }
            finally
            {
                DevConsole.Ansi(console =>
                {
                    console.Write(new Text("Exit code:", new Style(Color.Blue)));
                    console.Write(" ");
                    console.Write(new Text(exitCode.ToString(), new Style(exitCode == 0 ? Color.Default : Color.Red)));
                });
            }
            return exitCode;
        }
    }
}