using System;
using System.IO;

namespace DotNet.Basics.IO.Robust
{
    public static class LongPath
    {
        public static void CopyFile(string sourceFullPath, string destFullPath, bool overWrite)
        {
            NetCoreLongPath.Instance.Value.CopyFile(sourceFullPath, destFullPath, overWrite);
        }
        public static void CreateDir(string path)
        {
            NetCoreLongPath.Instance.Value.CreateDir(path);
        }

        public static bool Exists(string path)
        {
            return NetCoreLongPath.Instance.Value.Exists(path);
        }

        public static string GetFullName(string path)
        {
            return NetCoreLongPath.Instance.Value.NormalizePath(path);
        }

        public static void MoveFile(string sourceFullPath, string destFullPath)
        {
            NetCoreLongPath.Instance.Value.Movefile(sourceFullPath, destFullPath);
        }

        public static bool TryDelete(string fullPath)
        {
            Exception lastException = null;
            try
            {
                NetCoreLongPath.Instance.Value.DeleteDir(fullPath);
            }
            catch (Exception e)
            {
                lastException = e;
            }
            try
            {
                NetCoreLongPath.Instance.Value.DeleteFile(fullPath);
            }
            catch (Exception e)
            {
                lastException = e;
            }

            if (NetCoreLongPath.Instance.Value.Exists(fullPath))
                throw new IOException($"Failed to delete: {fullPath}", lastException);

            return true;
        }

    }
}
