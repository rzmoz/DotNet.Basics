using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class FileExtensions
    {
        public static bool IsFileType(this FileInfo file, FileType fileType)
        {
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            if (file == null)
                return false;
            return file.Name.EndsWith(fileType.Extension, true, null);
        }

        public static FileInfo ToFile(this string dir, params string[] paths)
        {
            var path = new Path(dir, false).Add(paths).ToString(PathDelimiter.Backslash);
            return new FileInfo(path);
        }

        public static FileInfo ToFile(this DirectoryInfo dir, params string[] paths)
        {
            return ToFile(dir.FullName, paths);
        }

        public static void CopyTo(this IEnumerable<FileInfo> sourceFiles, DirectoryInfo targetDir, bool overwrite = false)
        {
            var paths = sourceFiles.Select(file => file.FullName).ToArray();
            targetDir.CreateIfNotExists();
            PowerShellConsole.CopyItem(paths, targetDir.FullName, force: overwrite, recurse: false);
        }

        public static void CopyTo(this FileInfo file, DirectoryInfo targetDir, bool overwrite = false)
        {
            file.CopyTo(targetDir.ToFile(file.Name), overwrite: overwrite);
        }

        public static void CopyTo(this FileInfo source, DirectoryInfo targetDir, string newFileName, bool overwrite = false)
        {
            source.CopyTo(targetDir.ToFile(newFileName), overwrite: overwrite);
        }

        public static void CopyTo(this FileInfo source, FileInfo target, bool overwrite = false)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            target.Directory.CreateIfNotExists();
            PowerShellConsole.CopyItem(source.FullName, target.FullName, force: overwrite, recurse: false);
            Debug.WriteLine($"{source.FullName} copied to: {target.FullName}");
        }

        public static void MoveTo(this FileInfo source, DirectoryInfo targetDir, bool overwrite = false)
        {
            source.MoveTo(targetDir.ToFile(source.Name), overwrite);
        }
        public static void MoveTo(this FileInfo sourceFile, FileInfo targetFile, bool overwrite = false)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (sourceFile.FullName == targetFile.FullName)
            {
                Debug.WriteLine("MoveTo skipped. Source and target are the same: {0}", sourceFile.FullName);
                return;
            }

            if (!sourceFile.Exists())
            {
                Debug.WriteLine("MoveTo skipped. Source not found: {0}", sourceFile.FullName);
                return;
            }

            if (overwrite == false && targetFile.Exists())
            {
                Debug.WriteLine("MoveTo skipped. Target already exists and overwrite is set to false: {0}", targetFile.FullName);
                return;
            }
            targetFile.Directory.CreateIfNotExists();
            PowerShellConsole.MoveItem(sourceFile.FullName, targetFile.FullName, force: true);
        }

        public static string NameWithoutExtension(this FileInfo file)
        {
            return System.IO.Path.GetFileNameWithoutExtension(file?.Name);
        }

        public static string ReadAllText(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            return File.ReadAllText(file.FullName);
        }
    }
}