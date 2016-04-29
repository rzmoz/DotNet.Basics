using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace DotNet.Basics.Sys
{
    public static class PowerShellConsole
    {
        public static object[] RunScript(string script)
        {
            if (script == null) { throw new ArgumentNullException(nameof(script)); }
            using (var ps = PowerShell.Create())
            {
                BypassExecutionPolicyForProcessScope(ps);
                ps.AddScript(script);
                return WriteResult(ps);
            }
        }

        public static object[] RunFunction(string methodName, KeyValuePair<string, object> arg, string scriptPath)
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

                return WriteResult(ps);
            }
        }

        private static object[] WriteResult(PowerShell ps)
        {
            var results = ps.Invoke();
            return results.Select(pso => pso.BaseObject).ToArray();
        }


        private static void BypassExecutionPolicyForProcessScope(PowerShell ps)
        {
            ps.AddScript("Set-ExecutionPolicy Bypass -Scope Process");
        }
    }
}
