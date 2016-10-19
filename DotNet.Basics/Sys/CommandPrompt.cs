using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public static class CommandPrompt
    {
        public static int Run(string commandString)
        {
            DebugOut.WriteLine($"Command prompt invoked: {commandString}");

            var si = new ProcessStartInfo("cmd.exe", $"/c {commandString}")
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var console = new Process { StartInfo = si })
            {
                console.Start();

                DebugOut.WriteLine(console.StandardOutput.ReadToEnd());

                console.WaitForExit();
                var exitCode = console.ExitCode;

                DebugOut.WriteLine($"ExitCode:{exitCode} returned from {commandString}");

                console.Close();
                return exitCode;
            }
        }
    }
}
