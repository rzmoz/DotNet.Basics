using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public static class CommandPrompt
    {
        public delegate void OutputEventHandler(string message);

        public static event OutputEventHandler StandardOut;

        public static int Run(string commandString)
        {
            StandardOut?.Invoke($"Command prompt invoked: {commandString}");

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

                var output = console.StandardOutput.ReadToEnd();
                StandardOut?.Invoke(output);

                console.WaitForExit();

                var exitCode = console.ExitCode;
                StandardOut?.Invoke($"ExitCode:{exitCode} returned from {commandString}");
                console.Close();
                return exitCode;
            }
        }
    }
}
