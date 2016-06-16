using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class FileExtensions
    {
        public static FilePath ToFile(this string path, params string[] pathSegments)
        {
            return new FilePath(path).Add(pathSegments);
        }
        public static FilePath ToFile(this Path path, params string[] pathSegments)
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
            {
                Debug.WriteLine($"MoveTo skipped. Source and target are the same: {sourceFile.FullName}");
                return false;
            }

            if (sourceFile.Exists() == false)
            {
                Debug.WriteLine($"MoveTo skipped. Source not found: {sourceFile.FullName}");
                return false;
            }

            if (overwrite == false && targetFile.ToFile().Exists())
            {
                Debug.WriteLine($"MoveTo skipped. Target already exists and overwrite is set to false: {targetFile}");
                return false;
            }
            targetFile.Directory.CreateIfNotExists();
            PowerShellConsole.MoveItem(sourceFile.FullName, targetFile.FullName, force: true);
            return targetFile.Exists();
        }


        public static void CopyTo(this IEnumerable<FilePath> sourceFiles, DirPath targetDir, bool overwrite = false)
        {
            var paths = sourceFiles.Select(file => file.FullName).ToArray();
            targetDir.CreateIfNotExists();
            PowerShellConsole.CopyItem(paths, targetDir.FullName, force: overwrite, recurse: false);
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
            PowerShellConsole.CopyItem(source.FullName, target.FullName, force: overwrite, recurse: false);
            Debug.WriteLine($"{source.FullName} copied to: {target.FullName}");
        }
    }
}
