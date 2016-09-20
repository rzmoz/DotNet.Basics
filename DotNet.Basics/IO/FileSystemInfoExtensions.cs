using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class FileSystemInfoExtensions
    {
        public static void CopyTo(this Path path, Path destination, bool overwrite = false)
        {
            if (destination.IsFolder)
                path.CopyTo(destination.ToDir(), overwrite);
            else
                path.CopyTo(destination.ToFile(), overwrite);
        }

        public static void CopyTo(this Path path, DirPath destination, bool overwrite = false)
        {
            destination.CreateIfNotExists();

            if (path.IsFolder)
                Robocopy.CopyDir(path.FullName, destination.FullName, true);
            else
                PowerShellConsole.CopyItem(path.FullName, destination.FullName, force: false, recurse: false);
        }

        public static void CopyTo(this Path path, FilePath destination, bool overwrite = false)
        {
            if (path is DirPath)
                throw new ArgumentException("You're trying to copy a folder to a file. You should archive it (zip it)");
            PowerShellConsole.CopyItem(path.FullName, destination.FullName, force: overwrite, recurse: false);
        }
    }
}
