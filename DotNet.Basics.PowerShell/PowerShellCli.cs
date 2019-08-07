using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace DotNet.Basics.PowerShell
{
    public static class PowerShellCli
    {
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";

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

                using (var pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.AddScript(_bypassExecutionPolicy);
                    foreach (var script in scripts)
                        pipeline.Commands.AddScript(script);

                    var passThru = pipeline.Invoke();
                    runspace.Close();

                    if (pipeline.HadErrors)
                    {
                        if (!(pipeline.Error.Read() is PSObject errorsObject))
                            throw new ArgumentException("Unknown error in PowerShell script");

                        switch (errorsObject.BaseObject)
                        {
                            case ErrorRecord error when error.Exception != null:
                                throw error.Exception;
                            case ErrorRecord error:
                                throw new ArgumentException(error.ErrorDetails.Message);
                            case Collection<ErrorRecord> errors:
                                throw new AggregateException(errors.Select(e => e.Exception ?? new CmdletInvocationException(e.ErrorDetails.ToString())));
                        }
                    }
                    return passThru.Select(pt => pt.BaseObject).ToArray();
                }
            }
        }
    }
}
