using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class FileSystemInfoExtensions
    {
        public static DirectoryInfo Directory(this FileSystemInfo fsi)
        {
            if (fsi == null) throw new ArgumentNullException(nameof(fsi));
            if (fsi is DirectoryInfo)
                return fsi as DirectoryInfo;
            if (fsi is FileInfo)
                return (fsi as FileInfo).Directory;
            throw new ArgumentOutOfRangeException($"Type not supported: {fsi.GetType()}");
        }

        public static void CopyTo(this FileSystemInfo fsi, string destination)
        {
            var recurse = fsi is DirectoryInfo;
            PowerShellConsole.CopyItem(fsi.FullName, destination, force: true, recurse: recurse);
        }


        public static bool Exists(this FileSystemInfo fsi)
        {
            if (fsi == null)
                return false;
            fsi.Refresh();
            Debug.WriteLine($"{fsi.FullName} exists:{fsi.Exists}");
            return fsi.Exists;
        }

        public static bool DeleteIfExists(this FileSystemInfo fsi)
        {
            return DeleteIfExists(fsi, 30.Seconds());
        }
        public static bool DeleteIfExists(this FileSystemInfo fsi, TimeSpan timeout)
        {
            if (fsi == null)
                return false;

            Repeat.Task(() =>
            {
                PowerShellConsole.RemoveItem(fsi.FullName, force: true, recurse: true, errorAction: ActionPreference.SilentlyContinue);
            })
            .WithTimeout(timeout)
            .WithRetryDelay(3.Seconds())
            .Until(() => fsi.Exists() == false)
            .Now();

            return fsi.Exists() == false;
        }
    }
}
