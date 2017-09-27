using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class IoFilePathExtensions
    {
        public static void WriteAllText(this FilePath targetFile, string content, bool overwrite = true)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));
            if (overwrite == false && targetFile.Exists())
                throw new IOException($"Cannot write text. Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullPath()}");

            targetFile.Directory.CreateIfNotExists();
            File.WriteAllText(targetFile.FullPath(), content ?? string.Empty);
        }
    }
}
