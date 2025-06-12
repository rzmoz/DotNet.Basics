using System;
using System.ComponentModel;
using System.Diagnostics;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Sys
{
    public static class ExternalProcess
    {
        public static int Run(string path, string args, ILogger? logger = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var si = new ProcessStartInfo(path, args)
            {
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = new Process { StartInfo = si };

                process.OutputDataReceived += (_, data) =>
                {
                    if (data.Data != null) { logger?.Info(data.Data); }
                };

                process.ErrorDataReceived += (_, data) =>
                {
                    if (data.Data != null) { logger?.Error(data.Data); }
                };

                process.Start();
                logger?.Debug("[{processId}] Process <{ProcessName}> starting: {path} {args}", process.Id, process.ProcessName, path, args);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                logger?.Debug("[{processId}] Process <{processName}> Exit Code: {exitCode}", process.Id, process.ProcessName, process.ExitCode);

                return process.ExitCode;
            }
            catch (Win32Exception e)
            {
                logger?.Error("Failed to start process: {path} {args}", path, args, e);
                throw;
            }
        }
    }
}
