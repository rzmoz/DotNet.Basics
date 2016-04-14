using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public static class FileSystemInfoExtensions
    {
        public static bool Exists(this FileSystemInfo fsi)
        {
            if (fsi == null)
                return false;
            fsi.Refresh();
            Debug.WriteLine($"{fsi.FullName} exists:{fsi.Exists}");
            return fsi.Exists;
        }

        public static bool DeleteIfExists(this FileSystemInfo fsi)
        {
            if (fsi == null)
                return false;

            Repeat.Task(() =>
            {
                var psc = new PowerShellConsole();
                var cleanDirScript = $@"Remove-Item ""{fsi.FullName}"" -Recurse -Force -ErrorAction SilentlyContinue";
                psc.RunScript(cleanDirScript);
            })
            .WithTimeout(30.Seconds())
            .WithRetryDelay(3.Seconds())
            .Until(() => fsi.Exists() == false)
            .Now();

            return fsi.Exists() == false;
        }
    }
}
