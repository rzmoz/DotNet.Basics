using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class IoFilePathExtensions
    {
        public static FilePath WriteAllText(this FilePath targetFile, string content, bool overwrite = true)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (overwrite == false && targetFile.Exists())
                throw new IOException($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullPath()}");

            targetFile.Directory.CreateIfNotExists();
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
