using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public static class FileExtensions
    {
        public static FilePath ToFile(this string path, params string[] pathSegments)
        {
            return new FilePath(path).Add(pathSegments);
        }

        public static FilePath ToFile(this PathInfo path, params string[] pathSegments)
        {
            return new FilePath(path.Segments, path.Delimiter).Add(pathSegments);
        }

        public static bool MoveTo(this FilePath sourceFile, DirPath targetDir, bool overwrite = false)
        {
            return sourceFile.MoveTo(targetDir.ToFile(sourceFile.Name), overwrite);
        }

        public static bool MoveTo(this FilePath sourceFile, string targetFile, bool overwrite = false)
        {
            return sourceFile.MoveTo(targetFile.ToFile(), overwrite);
        }

        public static bool MoveTo(this FilePath sourceFile, FilePath targetFile, bool overwrite = false)
        {
            sourceFile.CopyTo(targetFile, overwrite);
            sourceFile.DeleteIfExists();
            return targetFile.Exists();
        }

        public static void CopyTo(this IEnumerable<FilePath> sourceFiles, DirPath targetDir, bool overwrite = false)
        {
            Parallel.ForEach(sourceFiles, sFile => sFile.CopyTo(targetDir.ToFile(sFile.Name), overwrite));
        }

        public static void CopyTo(this FilePath file, DirPath targetDir, bool overwrite = false)
        {
            file.CopyTo(targetDir.ToFile(file.Name), overwrite);
        }

        public static void CopyTo(this FilePath source, DirPath targetDir, string newFileName, bool overwrite = false)
        {
            source.CopyTo(targetDir.ToFile(newFileName), overwrite);
        }

        public static void CopyTo(this FilePath source, FilePath target, bool overwrite = false)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            target.Directory.CreateIfNotExists();
            //no logging here since it heavily impacts performance
            File.Copy(source.FullName, target.FullName, overwrite);
            File.SetAttributes(target.FullName, FileAttributes.Normal);
        }
    }
}
