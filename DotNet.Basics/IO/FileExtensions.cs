using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

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
            var targetFile = targetDir.Add(sourceFile.Name).ToFile();
            return sourceFile.MoveTo(targetFile, overwrite);
        }

        public static bool MoveTo(this FilePath sourceFile, string targetFile, bool overwrite = false)
        {
            return sourceFile.MoveTo(targetFile.ToFile(), overwrite);
        }

        public static bool MoveTo(this FilePath sourceFile, FilePath targetFile, bool overwrite = false)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (sourceFile.FullName.Equals(targetFile.FullName, StringComparison.OrdinalIgnoreCase))
                return false;
            if (sourceFile.Exists() == false)
                return false;
            if (overwrite == false && targetFile.ToFile().Exists())
                return false;
            targetFile.Directory.CreateIfNotExists();
            PowerShellConsole.MoveItem(sourceFile.FullName, targetFile.FullName, force: true);
            return targetFile.Exists();
        }

        public static void CopyTo(this IEnumerable<FilePath> sourceFiles, DirPath targetDir, bool overwrite = false)
        {
            targetDir.CreateIfNotExists();
            Parallel.ForEach(sourceFiles, sFile => sFile.CopyTo(targetDir.ToFile(sFile.Name), overwrite));
        }

        public static void CopyTo(this FilePath file, DirPath targetDir, bool overwrite = false)
        {
            file.CopyTo(targetDir.Add(file.Name).ToFile(), overwrite: overwrite);
        }

        public static void CopyTo(this FilePath source, DirPath targetDir, string newFileName, bool overwrite = false)
        {
            source.CopyTo(targetDir.Add(newFileName).ToFile(), overwrite: overwrite);
        }

        public static void CopyTo(this FilePath source, FilePath target, bool overwrite = false)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            target.Directory.CreateIfNotExists();
            File.Copy(source.FullName, target.FullName, overwrite);
        }
    }
}
