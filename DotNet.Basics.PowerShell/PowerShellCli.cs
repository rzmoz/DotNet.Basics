using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace DotNet.Basics.PowerShell
{
    public static class PowerShellCli
    {
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";

        static PowerShellCli()
        {
            //locate if PowerShell is installed
            var rootFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\PowerShell";
            if (Directory.Exists(rootFolder) == false)
                throw new IOException($@"PowerShell Core is not installed. Looking in {rootFolder}. Get it here: https://github.com/PowerShell/PowerShell/releases");

            var allVersions = Directory.GetDirectories(rootFolder);

            //find latest release version first or then try pre-release version
            var latestVersion = allVersions.Where(v => v.Contains('-') == false).OrderByDescending(d => d).FirstOrDefault() ??
                                allVersions.Where(v => v.Contains('-')).OrderByDescending(d => d).FirstOrDefault();

            if (latestVersion == null)
                throw new IOException($@"PowerShell Core is not installed. Looking in {rootFolder}. Get it here: https://github.com/PowerShell/PowerShell/releases");

            //all good
            PowerShellAssemblyLoadContextInitializer.SetPowerShellAssemblyLoadContext(latestVersion);
        }
        
        public static object[] RemoveItem(string path, bool force = false, bool recurse = false)
        {
            return RemoveItem(new[] { path }, force, recurse);
        }
        public static object[] RemoveItem(string[] paths, bool force = false, bool recurse = false)
        {
            var cmdlet = new PowerShellCmdlet("Remove-Item")
                .AddParameter("Path", paths)
                .WithForce(force)
                .WithRecurse(recurse);
            return RunScript(cmdlet.ToScript());
        }

        public static object[] RunScript(params string[] scripts)
        {
            if (scripts == null) { throw new ArgumentNullException(nameof(scripts)); }

            /*


            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                ps.AddScript("Get-Process | Out-String");
                var passThru = ps.Invoke();
                Console.WriteLine(passThru[0].ToString());
                return passThru.Select(pt => pt.BaseObject).ToArray();
            }*/

            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                var pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(_bypassExecutionPolicy);
                foreach (var script in scripts)
                {
                    pipeline.Commands.AddScript(script);
                }

                var passThru = pipeline.Invoke();
                runspace.Close();

                if (pipeline.HadErrors)
                {
                    if (!(pipeline.Error.Read() is PSObject errorsObject))
                        throw new ArgumentException("Unknown error in powershell script");

                    if (errorsObject.BaseObject is ErrorRecord error)
                    {
                        if (error.Exception != null)
                        {
                            throw error.Exception;
                        }
                        throw new ArgumentException(error.ErrorDetails.Message);
                    }

                    if (errorsObject.BaseObject is Collection<ErrorRecord> errors)
                    {
                        var errorMessage = errors.Aggregate(string.Empty, (current, err) => current + (err.Exception.ToString() + Environment.NewLine));
                        throw new ArgumentException(errorMessage);
                    }
                }
                return passThru.Select(pt => pt.BaseObject).ToArray();
            }
        }
    }
}
