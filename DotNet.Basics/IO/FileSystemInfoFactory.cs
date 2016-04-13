using System;
using System.IO;

namespace DotNet.Basics.IO
{
    public class FileSystemInfoFactory
    {
        public FileSystemInfo Create(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            try
            {

                FileAttributes attr = File.GetAttributes(path);

                if (attr.HasFlag(FileAttributes.Directory))
                    return new DirectoryInfo(path);
                else
                    return new FileInfo(path);
            }
            catch (PathTooLongException e)
            {
                throw new PathTooLongException(path, e);
            }
        }

    }
}
