namespace DotNet.Basics.IO
{
    public static class FileSystem
    {
        static FileSystem()
        {
            Current = new FileSystemBridge();
        }

        public static IFileSystem Current { get;}
    }
}
