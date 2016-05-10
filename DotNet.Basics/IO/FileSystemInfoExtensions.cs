using System;
using System.Diagnostics;
using System.IO;
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
            if (fsi == null)
                return false;

            Repeat.Task(() =>
            {
                var result = PowerShellConsole.RemoveItem(fsi.FullName, force: true, recurse: true, errorAction: "SilentlyContinue");
            })
            .WithTimeout(30.Seconds())
            .WithRetryDelay(3.Seconds())
            .Until(() => fsi.Exists() == false)
            .Now();

            return fsi.Exists() == false;
        }
    }
}
