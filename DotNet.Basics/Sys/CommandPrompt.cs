using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public static class CommandPrompt
    {
        public static int Run(string commandString)
        {
            Trace.WriteLine($"Command prompt invoked: {commandString}");

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

                Trace.WriteLine(console.StandardOutput.ReadToEnd());
                var error = console.StandardError.ReadToEnd();
                if (error.Length > 0)
                    Trace.WriteLine($"[Error]: {error}");

                var exitCode = console.ExitCode;
                Trace.WriteLine($"ExitCode:{exitCode} returned from {commandString}");
                console.Close();
                return exitCode;
            }
        }
    }
}
