using System.Diagnostics;
using System.IO;

namespace DotNet.Basics.IO
{
    public static class StringExtensions
    {
        public static void WriteToDisk(this string content, IoFile targetFile)
        {
            if (targetFile == null)
                return;

            targetFile.Directory.CreateIfNotExists();

            File.WriteAllText(targetFile.FullName, content);
            Debug.WriteLine($"Saved string to disk at: {targetFile.FullName}");
        }

        public static IoFile WriteToDisk(this string content, IoDir dir, string filename)
        {
            var file = new IoFile(dir, filename);
            WriteToDisk(content, file);
            return file;
        }
    }
}
