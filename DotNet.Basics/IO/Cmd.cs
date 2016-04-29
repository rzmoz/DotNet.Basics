using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class Cmd
    {
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
