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
            if (args.Any(a => a.EndsWith("-debug", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                Console.ReadLine();
            }

            var exitCode = int.MinValue;

            try
            {
                exitCode = await spectreApp.RunAsync(args);
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