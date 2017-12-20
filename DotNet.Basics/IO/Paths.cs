using System;

namespace DotNet.Basics.IO
{
    public static class Paths
    {
        public const string ExtendedPathPrefix = @"\\?\";

        static Paths()
        {
            try
            {
                UseFileSystem(new NetCoreWin32FileSystemLongPaths());
            }
            catch (Exception)
            {
                UseFileSystem(new NetFrameworkWin32FileSystemLongPaths());
            }
        }

        public static IFileSystem FileSystem { get; private set; }

        public static void UseFileSystem(IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
    }
}
