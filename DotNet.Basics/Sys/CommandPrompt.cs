using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Sys
{
    public static class CommandPrompt
    {
        public static int Run(string commandString, ILogger logger = null)
        {
            System.Console.WriteLine($"Command prompt invoked: {commandString}");

            var si = new ProcessStartInfo("cmd.exe", $"/c {commandString}")
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var console = new Process { StartInfo = si })
            {
                console.Start();

                logger?.LogTrace(console.StandardOutput.ReadToEnd());
                var error = console.StandardError.ReadToEnd();
                if (error.Length > 0)
                    logger?.LogError(error);

                var exitCode = console.ExitCode;
                logger?.LogTrace($"ExitCode:{exitCode}");
                console.Close();
                return exitCode;
            }
        }
    }
}
