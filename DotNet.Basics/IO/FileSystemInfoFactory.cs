using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class FileSystemInfoFactory
    {
        public T Create<T>(string path) where T : FileSystemInfo
        {
            try
            {
                var type = typeof(T);
                FileSystemInfo combined = null;
                if (type.Is<DirectoryInfo>())
                    combined = new DirectoryInfo(path);
                else if (type.Is<FileInfo>())
                    combined = new FileInfo(path);
                return (T)combined;
            }
            catch (PathTooLongException e)
            {
                throw new PathTooLongException(path, e);
            }
        }
    }
}
