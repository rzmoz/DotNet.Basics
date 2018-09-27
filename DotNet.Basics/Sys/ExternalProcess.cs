using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public class ExternalProcess
    {
        public static (string Input, int ExitCode, string Output) Run(string path, object args = null, bool useShellExecute = false, Action<string> logger = null)
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
                    logger?.Invoke($"Nee process started: [{process.Id}] {path} {args}");
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    logger?.Invoke($"Process [{process.Id}] exited with code: {process.ExitCode}");
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
