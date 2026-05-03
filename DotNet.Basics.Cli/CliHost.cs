using System;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Spectre.Console;
using DotNet.Basics.Cli.Logging;

namespace DotNet.Basics.Cli
{
    public class CliHost(CommandApp spectreApp)
    {
        public async Task<int> RunAsync(string[] args)
        {
            DevConsole.PauseForDebuggerAttach(args.Any(a => a.EndsWith("-debug", StringComparison.OrdinalIgnoreCase)));

            var exitCode = int.MinValue;

            try
            {
                exitCode = await spectreApp.RunAsync(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, new ExceptionSettings
                {
                    Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks,
                    Style = new ExceptionStyle
                    {
                        Exception = new Style(Color.White, Color.DarkRed),
                        Message = new Style(Color.Red),
                        LineNumber = new Style(Color.Blue),
                    }
                });
                exitCode = -1;
            }
            finally
            {
                DevConsole.Ansi(c =>
                {
                    c.Write(new Text("Exit code:", new Style(Color.Blue)));
                    c.Write(" ");
                    c.Write(new Text(exitCode.ToString(), new Style(exitCode == 0 ? Color.Default : Color.Red)));
                });
            }
            return exitCode;
        }
    }
}