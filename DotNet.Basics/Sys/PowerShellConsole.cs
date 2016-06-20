using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public class PowerShellConsole
    {
        public static object[] CopyItem(string path, string destination, bool force, bool recurse)
        {
            return CopyItem(path.ToEnumerable().ToArray(), destination, force, recurse);
        }
        public static object[] CopyItem(string[] paths, string destination, bool force, bool recurse)
        {
            var cmdlet = new PowerShellCmdlet("Copy-Item")
                .AddParameter("Path", paths)
                .AddParameter("Destination", destination)
                .WithForce(force)
                .WithRecurse(recurse);
            return RunScript(cmdlet.ToScript());
        }

        public static object[] RemoveItem(string path, bool force, bool recurse)
        {
            return RemoveItem(path.ToEnumerable().ToArray(), force, recurse);
        }
        public static object[] RemoveItem(string[] paths, bool force, bool recurse)
        {
            var cmdlet = new PowerShellCmdlet("Remove-Item")
                .AddParameter("Path", paths)
                .WithForce(force)
                .WithRecurse(recurse);
            return RunScript(cmdlet.ToScript());
        }

        public static object[] MoveItem(string path, string destination, bool force)
        {
            return MoveItem(path.ToEnumerable().ToArray(), destination, force);
        }
        public static object[] MoveItem(string[] paths, string destination, bool force)
        {
            var cmdlet = new PowerShellCmdlet("Move-Item")
                .AddParameter("Path", paths)
                .AddParameter("Destination", destination)
                .WithForce(force);
            return RunScript(cmdlet.ToScript());
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
                    Debug.WriteLine($"Adding script to ps pipeline: {script}");
                    pipeline.Commands.AddScript(script);
                }

                var passThru = pipeline.Invoke();
                runspace.Close();

                if (pipeline.HadErrors)
                {
                    var errorsObject = pipeline.Error.Read() as PSObject;
                    if (errorsObject == null)
                        throw new ArgumentException("Something went wrong - Move-Item code should be reworked");

                    var error = errorsObject.BaseObject as ErrorRecord;
                    if (error != null)
                    {
                        if (error.Exception != null)
                            throw error.Exception;
                        throw new ArgumentException(error.ErrorDetails.Message);
                    }

                    var errors = errorsObject.BaseObject as Collection<ErrorRecord>;

                    if (errors != null)
                    {
                        var errorMessage = errors.Aggregate(string.Empty, (current, err) => current + (err.Exception.ToString() + Environment.NewLine));
                        throw new ArgumentException(errorMessage);
                    }
                }
                return passThru.Select(pt => pt.BaseObject).ToArray();
            }
        }

        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";
    }
}
