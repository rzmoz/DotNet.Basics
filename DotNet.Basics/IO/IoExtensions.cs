using System;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class IoExtensions
    {
        public static FilePath WriteAllText(this string content, string targetFile, bool overwrite = false)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

            var outFileCmdlet = new PowerShellCmdlet("Out-File");
            outFileCmdlet.AddParameter("FilePath", targetFile);
            outFileCmdlet.AddParameter("inputobject", content);
            outFileCmdlet.AddParameter("NoNewline");

            targetFile.ToFile().Directory.CreateIfNotExists();
            var result = Repeat.Task(() => PowerShellConsole.RunScript(outFileCmdlet.ToScript()),
                new RepeatOptions
                {
                    RetryDelay = 1.Seconds(),
                    MaxTries = 10
                })
                .UntilNoExceptions();
            
            return targetFile.ToFile();
        }
        public static FilePath WriteAllText(this string content, Path targetPath, bool overwrite = false)
        {
            if (targetPath == null) throw new ArgumentNullException(nameof(targetPath));
            if (targetPath is FilePath == false)
                throw new ArgumentException($"target path is not a file:{targetPath}. Was: {targetPath.GetType()}");

            return WriteAllText(content, targetPath.FullName, overwrite);
        }
        public static FilePath WriteAllText(this string content, FilePath targetPath, bool overwrite = false)
        {
            return WriteAllText(content, targetPath.FullName, overwrite);
        }
        public static FilePath WriteAllText(this string content, DirPath targetDir, string fileName, bool overwrite = false)
        {
            return WriteAllText(content, targetDir.ToFile(fileName).FullName, overwrite);
        }
    }
}
