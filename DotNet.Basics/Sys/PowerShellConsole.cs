using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public static class PowerShellConsole
    {
        public static PSResult RemoveItem(string path, bool force, bool recurse, string errorAction = "SilentlyContinue")
        {
            return RemoveItem(path.ToEnumerable(), force, recurse, errorAction);
        }
        public static PSResult RemoveItem(IEnumerable<string> paths, bool force, bool recurse, string errorAction = "SilentlyContinue")
        {
            var script = $@"Remove-Item -path {paths.ToPSParamString()}"
                .WithForce(force)
                .WithRecurse(recurse)
                .WithErrorAction();

            return RunScript(script);
        }

        public static PSResult MoveItem(string path, string destination, bool force)
        {
            return MoveItem(path.ToEnumerable(), destination, force);
        }
        public static PSResult MoveItem(IEnumerable<string> paths, string destination, bool force)
        {
            var script = $@"Move-Item -path {paths.ToPSParamString()}"
                .WithForce(force)
                .WithErrorAction();

            return RunScript(script);
        }

        public static PSResult RunScript(string script)
        {
            if (script == null) { throw new ArgumentNullException(nameof(script)); }
            using (var ps = PowerShell.Create())
            {
                BypassExecutionPolicyForProcessScope(ps);
                ps.AddScript(script);
                return GetResult(ps);
            }
        }

        public static PSResult RunFunction(string methodName, KeyValuePair<string, object> arg, string scriptPath)
        {
            if (methodName == null) { throw new ArgumentNullException(nameof(methodName)); }
            if (scriptPath == null) { throw new ArgumentNullException(nameof(scriptPath)); }

            var file = new FileInfo(scriptPath);

            if (File.Exists(file.FullName) == false)
                throw new ArgumentException($"Script not found at:{file.FullName}");

            using (var ps = PowerShell.Create())
            {
                BypassExecutionPolicyForProcessScope(ps);
                ps.AddScript($". \"{file.FullName}\"");
                ps.Invoke();

                ps.Commands.Clear();

                BypassExecutionPolicyForProcessScope(ps);
                ps.AddCommand(methodName).AddParameter(arg.Key, arg.Value);

                return GetResult(ps);
            }
        }

        private static PSResult GetResult(PowerShell ps)
        {
            var psPassThru = ps.Invoke();
            var objectPassThru = psPassThru.Select(pso => pso.BaseObject).ToArray();
            return new PSResult(ps.HadErrors, objectPassThru);
        }

        private static void BypassExecutionPolicyForProcessScope(PowerShell ps)
        {
            ps.AddScript("Set-ExecutionPolicy Bypass -Scope Process");
        }
    }
}
