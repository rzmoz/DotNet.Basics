using System;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
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
                .WithRecurse(recurse)
                .WithErrorAction(ActionPreference.SilentlyContinue); ;
            return RunScript(cmdlet.ToScript());
        }

        public static object[] RenameItem(string path, string newName, bool force)
        {
            return RenameItem(path.ToEnumerable().ToArray(), newName, force);
        }
        public static object[] RenameItem(string[] paths, string newName, bool force)
        {
            var cmdlet = new PowerShellCmdlet("Rename-Item")
                .AddParameter("Path", paths)
                .AddParameter("NewName", newName)
                .WithForce(force)
                .WithErrorAction(ActionPreference.SilentlyContinue); ;
            return RunScript(cmdlet.ToScript());
        }

        public static object[] NewItem(string path, string itemType, bool force)
        {
            return NewItem(path.ToEnumerable().ToArray(), itemType, force);
        }
        public static object[] NewItem(string[] paths, string itemType, bool force)
        {
            var cmdlet = new PowerShellCmdlet("New-Item")
                .AddParameter("Path", paths)
                .AddParameter("ItemType", itemType)
                .WithForce(force)
                .WithErrorAction(ActionPreference.SilentlyContinue); ;
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
                .WithRecurse(recurse)
                .WithErrorAction(ActionPreference.SilentlyContinue);
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
                .WithForce(force)
                .WithErrorAction(ActionPreference.SilentlyContinue);
            return RunScript(cmdlet.ToScript());
        }


        public static object[] RunScript(string script)
        {
            if (script == null) { throw new ArgumentNullException(nameof(script)); }
            Debug.WriteLine($"Running script: {script}");
            using (var ps = PowerShell.Create())
            {
                ps.AddScript(_bypassExecutionPolicy);
                ps.AddScript(script);

                return Run(ps);
            }
        }

        private static object[] Run(PowerShell ps)
        {
            var passThrus = ps.Invoke();
            return passThrus.Select(pt => pt.BaseObject).ToArray();
        }

        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";
    }
}
