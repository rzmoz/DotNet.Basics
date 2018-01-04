using System;

namespace DotNet.Standard.IO
{
    public static class FileSystem
    {
        static FileSystem()
        {
            try
            {
                Current = new NetCoreWin32FileSystemLongPaths();
            }
            catch (Exception)
            {
                Current = new NetFrameworkWin32FileSystemLongPaths();
            }
        }

        public static IFileSystem Current { get; }
    }
}
