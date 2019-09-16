using System;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNet.Basics.Sys
{
    public static class ExternalProcess
    {
        public static int Run(string path, object args = null, Action<string> writeOutput = null, Action<string> writeError = null, Action<string> writeDebug = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var si = new ProcessStartInfo(path, args?.ToString() ?? string.Empty)
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
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
                    writeDebug?.Invoke($"[{process.Id}] Process <{process.ProcessName}> starting: {path} {args}");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    writeDebug?.Invoke($"[{process.Id}] Process <{process.ProcessName}> Exit Code: {process.ExitCode}");

                    return process.ExitCode;
                }
            }
            catch (Win32Exception e)
            {
                throw new Win32Exception($"Failed to start process: {path} {args}", e);
            }
        }
    }
}
