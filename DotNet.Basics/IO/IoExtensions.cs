using System;
using System.Diagnostics;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class IoExtensions
    {
        public static Path WriteAllText(this string content, Path targetFile, bool overwrite = false)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

            var outFileCmdlet = new PowerShellCmdlet("Out-File");
            outFileCmdlet.AddParameter("FilePath", targetFile.FullName);
            outFileCmdlet.AddParameter("inputobject", content);
            outFileCmdlet.AddParameter("NoNewline");

            targetFile.Directory.CreateIfNotExists();
            var result = Repeat.Task(() => PowerShellConsole.RunScript(outFileCmdlet.ToScript()))
                .WithRetryDelay(1.Seconds())
                .UntilNoExceptions()
                .WithMaxTries(10)
                .Now();

            Debug.WriteLine($"Saved text to disk: {targetFile.FullName}");
            return targetFile;
        }

        public static FileInfo WriteAllText(this string content, FileInfo targetFile)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

            return content.WriteAllText(targetFile.FullName.ToPath(false)).ToFile();
        }

        public static FileInfo WriteAllText(this string content, DirectoryInfo dir, string filename)
        {
            var file = dir.ToFile(filename);
            WriteAllText(content, file);
            return file;
        }
    }
}
