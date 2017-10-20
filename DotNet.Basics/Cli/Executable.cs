using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNet.Basics.Cli
{
    public class Executable
    {
        public static (int ExitCode, string Output) Run(string path, object args = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var si = new ProcessStartInfo(path, args?.ToString() ?? string.Empty)
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (var process = new Process { StartInfo = si })
                {
                    process.Start();
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return (process.ExitCode, result.TrimEnd(' ', '\r', '\n'));
                }
            }
            catch (Win32Exception e)
            {
                throw new Win32Exception($"Failed to start process: {path} {args}", e);
            }
        }
    }
}
