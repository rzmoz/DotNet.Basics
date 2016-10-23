using System;
using System.IO;

namespace DotNet.Basics.IO
{
    public static class IoExtensions
    {
        public static bool WriteAllText(this string content, FilePath targetFile, bool overwrite = false)
        {
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

            if (overwrite == false && targetFile.Exists())
                return false;

            targetFile.Directory.CreateIfNotExists();
            File.WriteAllText(targetFile.FullName, content ?? string.Empty);
            return true;
        }

        public static bool WriteAllText(this string content, PathInfo target, bool overwrite = false)
        {
            return !target.IsFolder && content.WriteAllText(target as FilePath, overwrite);
        }
    }
}
