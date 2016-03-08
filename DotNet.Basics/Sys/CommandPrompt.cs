using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public static class CommandPrompt
    {
        public static int Run(string commandString)
        {
            System.Console.WriteLine("Command prompt invoked: {0}", commandString);

            var si = new ProcessStartInfo("cmd.exe", "/c " + commandString)
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

                System.Console.WriteLine(console.StandardOutput.ReadToEnd());
                var error = console.StandardError.ReadToEnd();
                if (error.Length > 0)
                    System.Console.WriteLine(error);
                var exitCode = console.ExitCode;
                System.Console.WriteLine("ExitCode:" + exitCode);
                console.Close();
                return exitCode;
            }
        }
    }
}
