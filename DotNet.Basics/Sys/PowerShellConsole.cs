using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace DotNet.Basics.Sys
{
    public class PowerShellConsole
    {
        public string RunScript(string script)
        {
            if (script == null) { throw new ArgumentNullException(nameof(script)); }
            using (var ps = PowerShell.Create())
            {
                BypassExecutionPolicyForProcessScope(ps);
                ps.AddScript(script);
                return WriteResult(ps);
            }
        }

        public string RunFunction(string methodName, KeyValuePair<string, object> arg, string scriptPath)
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

        private static string WriteResult(PowerShell ps)
        {
            var results = ps.Invoke();

            var resultString = new StringBuilder();

            foreach (var psObject in results)
                resultString.Append(psObject + Environment.NewLine);
            
            var result = resultString.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            Debug.WriteLine(result);
            return result;
        }


        private void BypassExecutionPolicyForProcessScope(PowerShell ps)
        {
            ps.AddScript("Set-ExecutionPolicy Bypass -Scope Process");
        }
    }
}
