using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO.Robust
{
    public static class FilePathExtensions
    {
        public static bool MoveTo(this FilePath fp, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            return fp.MoveTo(targetDir.ToFile(fp.Name), overwrite, ensureTargetDir);
        }

        public static bool MoveTo(this FilePath fp, string targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            return fp.MoveTo(targetFile.ToFile(), overwrite, ensureTargetDir);
        }

        public static bool MoveTo(this FilePath fp, FilePath targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (ensureTargetDir)
                targetFile.Directory().CreateIfNotExists();
            if (overwrite)
                targetFile.DeleteIfExists();

            LongPath.MoveFile(fp.FullName(), targetFile.FullName());
            return targetFile.Exists();
        }

        public static void CopyTo(this FilePath fp, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            fp.CopyTo(targetDir.ToFile(fp.Name), overwrite, ensureTargetDir);
        }

        public static void CopyTo(this FilePath fp, DirPath targetDir, string newFileName, bool overwrite = false, bool ensureTargetDir = true)
        {
            fp.CopyTo(targetDir.ToFile(newFileName), overwrite, ensureTargetDir);
        }

        public static void CopyTo(this FilePath fp, FilePath target, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (ensureTargetDir)
                target.Directory().CreateIfNotExists();

            LongPath.CopyFile(fp.FullName(), target.FullName(), overwrite);
        }

        public static bool IsFileType(this FilePath fp, FileType fileType)
        {
            return fileType != null && fp.Name.EndsWith(fileType.Extension, true, null);
        }

        public static FilePath WriteAllText(this FilePath fp, string content, bool overwrite = true)
        {
            if (overwrite == false && fp.Exists())
                throw new IOException($"Target file already exists. Set overwrite to true to ignore existing file: {fp.FullName()}");

            fp.Directory().CreateIfNotExists();
            File.WriteAllText(fp.FullName(), content ?? string.Empty);
            return fp;
        }
        public static string ReadAllText(this FilePath fp, IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            try
            {
                return File.ReadAllText(fp.FullName());
            }
            catch (DirectoryNotFoundException)
            {
                if (ifNotExists == IfNotExists.ThrowIoException)
                    throw;
            }
            catch (FileNotFoundException)
            {
                if (ifNotExists == IfNotExists.ThrowIoException)
                    throw;
            }
            return null;
        }
    }
}
