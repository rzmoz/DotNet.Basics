﻿using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
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

            File.Move(fp.FullName(), targetFile.FullName());
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
            if (fp == null) throw new ArgumentNullException(nameof(fp));
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (fp.Exists() == false)
                throw new FileNotFoundException(fp.FullName());
            if (ensureTargetDir)
                target.Directory().CreateIfNotExists();

            File.Copy(fp.FullName(), target.FullName(), overwrite);
        }

        public static FilePath WriteAllText(this FilePath fp, string content, bool overwrite = true)
        {
            if (overwrite == false && fp.Exists())
                throw new IOException($"Target file already exists. Set overwrite to true to ignore existing file: {fp.FullName()}");

            fp.Directory().CreateIfNotExists();

            //get filePath that's definitely not too long
            var testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, content ?? string.Empty);
            fp.DeleteIfExists();
            File.Move(testFile, fp.FullName());
            return fp;
        }
        public static byte[] ReadAllBytes(this FilePath fp, IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            try
            {
                return File.ReadAllBytes(fp.FullName());
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
        public static string ReadAllText(this FilePath fp, IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            try
            {
                return File.ReadAllText(fp.FullName()).EnsureNewlineHasCarriageReturn();
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

        public static FileStream Create(this FilePath fp, IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            try
            {
                return File.Create(fp.FullName());
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
        public static FileStream Create(this FilePath fp, Stream content, IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            var fileStream = Create(fp, ifNotExists);
            content.CopyTo(fileStream);
            fileStream.Flush();
            return fileStream;
        }

        public static StreamReader OpenRead(this FilePath fp, IfNotExists ifNotExists = IfNotExists.ThrowIoException, FileShare fileShare = FileShare.ReadWrite)
        {
            try
            {
                return new StreamReader(File.Open(fp.FullName(), FileMode.Open, FileAccess.Read, fileShare));
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

        /// <summary>
        /// Opens an existing file or creates a new file for writing.
        /// </summary>
        /// <param name="fp"></param>
        /// <returns></returns>
        public static StreamWriter OpenWrite(this FilePath fp, FileShare fileShare = FileShare.ReadWrite)
        {
            return new StreamWriter(File.Open(fp.FullName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, fileShare));
        }
    }
}
