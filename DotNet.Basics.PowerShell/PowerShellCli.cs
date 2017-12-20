using System;
using System.Collections.ObjectModel;
using System.Linq;
/*
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace DotNet.Basics.PowerShell
{
    public static class PowerShellCli
    {
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";

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
            return RunScript(cmdlet.ToString());
        }

        public static object[] RunCmdlet(PowerShellCmdlet cmdLet)
        {
            return RunScript(cmdLet.ToString());
        }

        public static object[] RunScript(params string[] scripts)
        {
            if (scripts == null) { throw new ArgumentNullException(nameof(scripts)); }

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
*/