using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class FilePath : PathInfo
    {
        public FilePath(string path, params string[] segments)
            : this(path, PathSeparator.Unknown, segments)
        {
        }

        public FilePath(string path, char pathSeparator, params string[] segments)
            : base(path, IO.IsFolder.False, pathSeparator, segments)
        {
            NameWoExtension = Path.GetFileNameWithoutExtension(Name);
            Extension = Path.GetExtension(Name);
        }
        public string NameWoExtension { get; }
        public string Extension { get; }

        public bool MoveTo(DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            return MoveTo(targetDir.ToFile(Name), overwrite, ensureTargetDir);
        }

        public bool MoveTo(string targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            return MoveTo(targetFile.ToFile(), overwrite, ensureTargetDir);
        }

        public bool MoveTo(FilePath targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            CopyTo(targetFile, overwrite, ensureTargetDir);
            DeleteIfExists();
            return targetFile.Exists();
        }

        public void CopyTo(DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            CopyTo(targetDir.ToFile(Name), overwrite, ensureTargetDir);
        }

        public void CopyTo(DirPath targetDir, string newFileName, bool overwrite = false, bool ensureTargetDir = true)
        {
            CopyTo(targetDir.ToFile(newFileName), overwrite, ensureTargetDir);
        }

        public void CopyTo(FilePath target, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (ensureTargetDir)
                target.Directory().CreateIfNotExists();
            //no logging here since it heavily impacts performance
            File.Copy(FullPath(), target.FullPath(), overwrite);
            File.SetAttributes(target.FullPath(), FileAttributes.Normal);
        }

        public bool IsFileType(FileType fileType)
        {
            if (fileType == null)
                return false;
            return Name.EndsWith(fileType.Extension, true, null);
        }

        public FilePath WriteAllText(string content, bool overwrite = true)
        {
            if (overwrite == false && Exists())
                throw new IOException($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {FullPath()}");

            this.Directory().CreateIfNotExists();
            File.WriteAllText(this.FullPath(), content ?? string.Empty);
            return this;
        }
        public string ReadAllText(IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            try
            {
                return File.ReadAllText(FullPath());
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
