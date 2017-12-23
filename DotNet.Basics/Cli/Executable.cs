using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public class Executable
    {
        public static (string Input, int ExitCode, string Output) Run(string path, object args = null, bool useShellExecute = false, ILogger logger = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var si = new ProcessStartInfo(path, args?.ToString() ?? string.Empty)
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = useShellExecute,
                CreateNoWindow = true
            };

            try
            {
                using (var process = new Process { StartInfo = si })
                {
                    process.Start();
                    logger?.LogTrace($"Nee process started: [{process.Id}] {path} {args}");
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    logger?.LogTrace($"Process [{process.Id}] exited with code: {process.ExitCode}");
                    return ($"{path} {args}", process.ExitCode, result.TrimEnd(' ', '\r', '\n'));
                }
            }
            catch (Win32Exception e)
            {
                throw new Win32Exception($"Failed to start process: {path} {args}", e);
            }
        }
    }
}
