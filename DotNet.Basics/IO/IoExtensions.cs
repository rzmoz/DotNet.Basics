using System;
using System.IO;

namespace DotNet.Basics.IO
{
    public static class IoExtensions
    {
        public static void WriteAllText(this string content, FilePath targetFile, bool overwrite = true)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (overwrite == false && targetFile.Exists())
                throw new IOException($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {targetFile}");

            targetFile.Directory.CreateIfNotExists();
            File.WriteAllText(targetFile.FullName, content ?? string.Empty);
        }

        public static void WriteAllText(this string content, PathInfo target, bool overwrite = true)
        {
            if (target.IsFolder)
                throw new ArgumentException($"Cannot write text. Target is a folder: {target}");

            content.WriteAllText(target as FilePath, overwrite);
        }
    }
}
