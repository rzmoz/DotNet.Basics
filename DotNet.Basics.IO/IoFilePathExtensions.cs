using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class IoFilePathExtensions
    {
        public static bool MoveTo(this FilePath sourceFile, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            return sourceFile.MoveTo(targetDir.ToFile(sourceFile.Name), overwrite, ensureTargetDir);
        }

        public static bool MoveTo(this FilePath sourceFile, string targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            return sourceFile.MoveTo(targetFile.ToFile(), overwrite, ensureTargetDir);
        }

        public static bool MoveTo(this FilePath sourceFile, FilePath targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            sourceFile.CopyTo(targetFile, overwrite, ensureTargetDir);
            sourceFile.DeleteIfExists();
            return targetFile.Exists();
        }

        public static void CopyTo(this IEnumerable<FilePath> sourceFiles, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (ensureTargetDir)
                targetDir.CreateIfNotExists();
            Parallel.ForEach(sourceFiles, sFile => sFile.CopyTo(targetDir.ToFile(sFile.Name), overwrite, ensureTargetDir: false));
        }

        public static void CopyTo(this FilePath file, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            file.CopyTo(targetDir.ToFile(file.Name), overwrite, ensureTargetDir);
        }

        public static void CopyTo(this FilePath source, DirPath targetDir, string newFileName, bool overwrite = false, bool ensureTargetDir = true)
        {
            source.CopyTo(targetDir.ToFile(newFileName), overwrite, ensureTargetDir);
        }

        public static void CopyTo(this FilePath source, FilePath target, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (ensureTargetDir)
                target.Directory().CreateIfNotExists();
            //no logging here since it heavily impacts performance
            File.Copy(source.FullPath(), target.FullPath(), overwrite);
            File.SetAttributes(target.FullPath(), FileAttributes.Normal);
        }

        public static FilePath WriteAllText(this FilePath targetFile, string content, bool overwrite = true)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (overwrite == false && targetFile.Exists())
                throw new IOException($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullPath()}");

            targetFile.Directory().CreateIfNotExists();
            File.WriteAllText(targetFile.FullPath(), content ?? string.Empty);
            return targetFile;
        }
        public static string ReadAllText(this FilePath file, bool throwIfNotExists = true)
        {
            try
            {
                return File.ReadAllText(file.FullPath());
            }
            catch (DirectoryNotFoundException)
            {
                if (throwIfNotExists)
                    throw;
            }
            catch (FileNotFoundException)
            {
                if (throwIfNotExists)
                    throw;
            }
            return null;
        }
    }
}
