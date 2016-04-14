using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class Cmd
    {
        public static int Move(string source, string target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var moveString = $@"move ""{source.TrimEnd('\\')}"" ""{target.TrimEnd('\\')}""";

            return CommandPrompt.Run(moveString);
        }

        public static int Move(DirectoryInfo source, DirectoryInfo target)
        {
            return Move(source.FullName, target.FullName);
        }
        public static int Move(FileInfo source, FileInfo target)
        {
            return Move(source.FullName, target.FullName);
        }

        public static int RmDir(string dir)
        {
            if (dir == null) { throw new ArgumentNullException(nameof(dir)); }

            var copyString = $@"rmdir /s /q ""{dir.TrimEnd('\\')}""";

            return CommandPrompt.Run(copyString);
        }

        public static int RmDir(DirectoryInfo dir)
        {
            if (dir == null) { throw new ArgumentNullException(nameof(dir)); }
            return RmDir(dir.FullName);
        }

        public static int MkLink(DirectoryInfo linkDir, DirectoryInfo targetDir)
        {
            if (linkDir == null) { throw new ArgumentNullException(nameof(linkDir)); }
            if (targetDir == null) { throw new ArgumentNullException(nameof(targetDir)); }

            if (linkDir.Exists())
                throw new IOException("link dir: '" + linkDir.FullName + "' already exists!");

            if (targetDir.Exists() == false)
                throw new IOException("target dir: '" + linkDir.FullName + "' doesn't exists!");

            var mkLinkString = $@"mklink /D ""{linkDir.FullName}"" ""{targetDir.FullName}""";
            return CommandPrompt.Run(mkLinkString);
        }
    }
}
