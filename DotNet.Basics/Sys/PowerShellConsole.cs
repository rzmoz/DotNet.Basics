using System;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public static class PowerShellConsole
    {
        private const string _silentlyContinueErrorAction = "SilentlyContinue";

        public static PowerShellResult CopyItem(string path, string destination, bool force, bool recurse)
        {
            return CopyItem(path.ToEnumerable().ToArray(), destination, force, recurse);
        }
        public static PowerShellResult CopyItem(string[] paths, string destination, bool force, bool recurse)
        {
            var cmdlet = new PowerShellCmdlet("Copy-Item")
                .AddParameter("Path", paths)
                .AddParameter("Destination", destination)
                .WithForce(force)
                .WithRecurse(recurse);
            return RunScript(cmdlet.ToScript());
        }

        public static PowerShellResult RenameItem(string path, string newName, bool force)
        {
            return RenameItem(path.ToEnumerable().ToArray(), newName, force);
        }
        public static PowerShellResult RenameItem(string[] paths, string newName, bool force)
        {
            var cmdlet = new PowerShellCmdlet("Rename-Item")
                .AddParameter("Path", paths)
                .AddParameter("NewName", newName)
                .WithForce(force);
            return RunScript(cmdlet.ToScript());
        }

        public static PowerShellResult NewItem(string path, string itemType, bool force)
        {
            return NewItem(path.ToEnumerable().ToArray(), itemType, force);
        }
        public static PowerShellResult NewItem(string[] paths, string itemType, bool force)
        {
            var cmdlet = new PowerShellCmdlet("New-Item")
                .AddParameter("Path", paths)
                .AddParameter("ItemType", itemType)
                .WithForce(force);
            return RunScript(cmdlet.ToScript());
        }

        public static PowerShellResult RemoveItem(string path, bool force, bool recurse, string errorAction = _silentlyContinueErrorAction)
        {
            return RemoveItem(path.ToEnumerable().ToArray(), force, recurse, errorAction);
        }
        public static PowerShellResult RemoveItem(string[] paths, bool force, bool recurse, string errorAction = _silentlyContinueErrorAction)
        {
            var cmdlet = new PowerShellCmdlet("Remove-Item")
                .AddParameter("Path", paths)
                .WithForce(force)
                .WithRecurse(recurse)
                .WithErrorAction(errorAction);
            return RunScript(cmdlet.ToScript());
        }

        public static PowerShellResult MoveItem(string path, string destination, bool force, string errorAction = _silentlyContinueErrorAction)
        {
            return MoveItem(path.ToEnumerable().ToArray(), destination, force, errorAction);
        }
        public static PowerShellResult MoveItem(string[] paths, string destination, bool force, string errorAction = _silentlyContinueErrorAction)
        {
            var cmdlet = new PowerShellCmdlet("Move-Item")
                .AddParameter("Path", paths)
                .AddParameter("Destination", destination)
                .WithForce(force)
                .WithErrorAction(errorAction);
            return RunScript(cmdlet.ToScript());
        }

        public static PowerShellResult RunScript(string script)
        {
            if (script == null) { throw new ArgumentNullException(nameof(script)); }
            Debug.WriteLine($"Running script: {script}");
            using (var ps = PowerShell.Create())
            {
                ps.AddScript(_bypassExecutionPolicy);
                ps.AddScript(script);

                var passThrus = ps.Invoke();
                return new PowerShellResult(true, passThrus.Select(pt => pt.BaseObject));
            }
        }
        private const string _bypassExecutionPolicy = "Set-ExecutionPolicy Bypass -Scope Process";
    }
}
