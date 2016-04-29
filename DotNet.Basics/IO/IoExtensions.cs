using System;
using System.Diagnostics;
using System.IO;

namespace DotNet.Basics.IO
{
    public static class IoExtensions
    {
        public static FileInfo WriteAllText(this string content, FileInfo targetFile)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (targetFile == null)
                throw new ArgumentNullException(nameof(targetFile));

            targetFile.Directory.CreateIfNotExists();

            File.WriteAllText(targetFile.FullName, content);
            Debug.WriteLine($"Saved string to disk at: {targetFile.FullName}");
            return targetFile;
        }

        public static FileInfo WriteAllText(this string content, DirectoryInfo dir, string filename)
        {
            var file = dir.ToFile(filename);
            WriteAllText(content, file);
            return file;
        }
    }
}
