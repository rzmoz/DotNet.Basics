using System.Diagnostics;
using NLog;

namespace DotNet.Basics.Sys
{
    public static class CommandPrompt
    {
        public static int Run(string commandString, ILogger logger = null)
        {
            if (logger == null)
                logger = LogManager.GetCurrentClassLogger();

            logger?.Trace($"Command prompt invoked: {commandString}");

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

                logger?.Trace(console.StandardOutput.ReadToEnd());
                var error = console.StandardError.ReadToEnd();
                if (error.Length > 0)
                    logger?.Error(error);

                var exitCode = console.ExitCode;
                logger?.Debug($"ExitCode:{exitCode}");
                console.Close();
                return exitCode;
            }
        }
    }
}
