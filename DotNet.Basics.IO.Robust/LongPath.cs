namespace DotNet.Basics.IO.Robust
{
    public static class LongPath
    {
        public static bool Exists(string path)
        {
#if NETSTANDARD2_0
            return NetStandardLongPath.Instance.Value.Exists(path);
#endif
#if net45
            return NetFrameworkLongPath.Instance.Value.Exists(path);
#endif      
        }

        public static string GetFullName(string path)
        {
#if NETSTANDARD2_0
            return NetStandardLongPath.Instance.Value.GetFullPath(path);
#endif
#if net45
            return NetFrameworkLongPath.Instance.Value.GetFullPath(path);
#endif
        }

        public static bool TryDelete(string path)
        {
            var fullPath = GetFullName(path);
            bool deletedDir;
            bool deletedFile;
#if NETSTANDARD2_0
            deletedDir = NetStandardLongPath.Instance.Value.TryDeleteDir(fullPath);
            deletedFile = NetStandardLongPath.Instance.Value.TryDeleteFile(fullPath);
#endif
#if net45
            deletedDir = NetFrameworkLongPath.Instance.Value.TryDeleteDir(fullPath);
            deletedFile = NetFrameworkLongPath.Instance.Value.TryDeleteFile(fullPath);
#endif
            return deletedDir || deletedFile;
        }

    }
}
