using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public static class FileExtensions
    {
        public static IoFile ToFile(this string root, params string[] paths)
        {
            return new IoFile(root, paths);
        }
        public static IoFile ToFile(this IoDir dir, string fileName)
        {
            return new IoFile(dir, fileName);
        }

        public static IoFile ToFile(this IoDir dir, IoFile fileName)
        {
            return new IoFile(dir, fileName.Name);
        }

        public static void CopyTo(this IEnumerable<IoFile> sourceFiles, IoDir targetDir, FileCopyOptions fileCopyOptions = FileCopyOptions.AbortIfExists)
        {
            Parallel.ForEach(sourceFiles, (file) => file.CopyTo(targetDir, fileCopyOptions));
        }
    }
}