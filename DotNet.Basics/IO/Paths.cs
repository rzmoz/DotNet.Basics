using System;

namespace DotNet.Basics.IO
{
    public static class Paths
    {
        static Paths()
        {
#if NETSTANDARD2_0
            UseNetCoreWin32LongPaths();
#endif
#if NET45
            UseNetFrameworkWin32LongPaths();
#endif
        }

        public static IFileSystem FileSystem { get; private set; }

        public static void UseFileSystem(IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
        public static void UseNetCoreWin32LongPaths()
        {
            FileSystem = new NetCoreWin32FileSystemLongPath();
        }
        public static void UseNetFrameworkWin32LongPaths()
        {
            FileSystem = new NetFrameworkWin32LongPath();
        }
    }
}
