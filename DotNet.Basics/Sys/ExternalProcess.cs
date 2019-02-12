using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public static class ExternalProcess
    {
        public static (string Input, int ExitCode) Run(string path, object args = null, Action<string> writeOutput = null, Action<string> writeError = null, bool useShellExecute = false)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var si = new ProcessStartInfo(path, args?.ToString() ?? string.Empty)
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = useShellExecute,
                CreateNoWindow = true
            };

            try
            {
                using (var process = new Process { StartInfo = si })
                {
                    if (writeOutput != null)
                        process.OutputDataReceived += (sender, data) =>
                        {
                            if (data.Data != null) { writeOutput.Invoke(data.Data); }
                        };
                    if (writeError != null)
                        process.ErrorDataReceived += (sender, data) =>
                        {
                            if (data.Data != null) { writeError.Invoke(data.Data); }
                        };
                    process.Start();
                    writeOutput?.Invoke($"Process started: [{process.Id}] {path} {args}");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    writeOutput?.Invoke($"Process [{process.Id}] exited with code: {process.ExitCode}");
                    return ($"{path} {args}", process.ExitCode);
                }
            }
            catch (Win32Exception e)
            {
                throw new Win32Exception($"Failed to start process: {path} {args}", e);
            }
        }
    }
}
