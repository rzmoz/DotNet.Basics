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

        public static void CopyTo(this FileSystemInfo fsi, FileSystemInfo destination, bool overwrite = false)
        {
            var info = destination as DirectoryInfo;
            if (info == null)
                fsi.CopyTo(destination as FileInfo, overwrite);
            else
                fsi.CopyTo(info, overwrite);
        }

        public static void CopyTo(this FileSystemInfo fsi, DirectoryInfo destination, bool overwrite = false)
        {
            var sourceIsFolder = fsi is DirectoryInfo;
            destination.CreateIfNotExists();

            if (sourceIsFolder)
                Robocopy.CopyDir(fsi.FullName, destination.FullName, true);
            else
                PowerShellConsole.CopyItem(fsi.FullName, destination.FullName, force: false, recurse: false);
        }
        public static void CopyTo(this FileSystemInfo fsi, FileInfo destination, bool overwrite = false)
        {
            if (fsi is DirectoryInfo)
                throw new ArgumentException("You're trying to copy a folder to a file. You should archive it (zip it)");
            PowerShellConsole.CopyItem(fsi.FullName, destination.FullName, force: overwrite, recurse: false);
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
                PowerShellConsole.RemoveItem(fsi.FullName, force: true, recurse: true);
            })
            .WithTimeout(timeout)
            .WithRetryDelay(3.Seconds())
            .Until(() => fsi.Exists() == false)
            .Now();

            return fsi.Exists() == false;
        }
    }
}
