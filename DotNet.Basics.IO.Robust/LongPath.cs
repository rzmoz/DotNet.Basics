using System;
using System.IO;

namespace DotNet.Basics.IO.Robust
{
    public static class LongPath
    {
        public static void CreateDir(string path)
        {
            NetStandardLongPath.Instance.Value.CreateDir(path);
        }

        public static bool Exists(string path)
        {
            return NetStandardLongPath.Instance.Value.Exists(path);
        }

        public static string GetFullName(string path)
        {
            return NetStandardLongPath.Instance.Value.NormalizePath(path);
        }

        public static void MoveFile(string sourceFullPath, string destFullPath)
        {
            NetStandardLongPath.Instance.Value.Movefile(sourceFullPath, destFullPath);
        }

        public static bool TryDelete(string fullPath)
        {
            Exception lastException = null;
            try
            {
                NetStandardLongPath.Instance.Value.DeleteDir(fullPath);
            }
            catch (Exception e)
            {
                lastException = e;
            }
            try
            {
                NetStandardLongPath.Instance.Value.DeleteFile(fullPath);
            }
            catch (Exception e)
            {
                lastException = e;
            }

            if (NetStandardLongPath.Instance.Value.Exists(fullPath))
                throw new IOException($"Failed to delete: {fullPath}", lastException);

            return true;
        }

    }
}
