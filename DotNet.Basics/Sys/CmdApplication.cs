using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Basics.IO;

namespace DotNet.Basics.Sys
{
    public class CmdApplication
    {
        private const string _installingHandleName = "installing.dat";
        private const string _installedHandleName = "installed.dat";
        private readonly FilePath _installedHandle;
        private readonly IList<Action> _installActions;

        public CmdApplication(string appName, string executableFilename, Stream content, bool disposeStreamWhenDone = true)
        : this(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToDir(appName),
            executableFilename, content, disposeStreamWhenDone)
        { }

        public CmdApplication(DirPath installDir, string executableFilename, Stream content, bool disposeStreamWhenDone = true)
        {
            InstallDir = installDir ?? throw new ArgumentNullException(nameof(installDir));
            EntryFile = installDir.ToFile(executableFilename);
            _installedHandle = installDir.ToFile(_installedHandleName);
            _installActions = new List<Action>();
            WithFile(executableFilename, content, disposeStreamWhenDone);
        }

        public DirPath InstallDir { get; }
        public FilePath EntryFile { get; }

        public CmdApplication WithFile(string filename, Stream content, bool disposeStreamWhenDone = true)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var target = InstallDir.ToFile(filename);
            _installActions.Add(() =>
            {
                target.DeleteIfExists();//we ensure file integrity if we got this far. No guarantees that corrupt files haven't been left behind by a faulty installation
                using (var fsDst = new FileStream(target.FullName(), FileMode.Create, FileAccess.Write))
                    content.CopyTo(fsDst);

                if (disposeStreamWhenDone)
                    content.Dispose();
            });
            return this;
        }

        public (string Input, int ExitCode, string Output) Run(params string[] args)
        {
            Install();
            var argString = args.Aggregate(string.Empty, (current, param) => current + $" {param}");
            return CmdPrompt.Run($"{EntryFile.FullName()} {argString}");
        }

        public bool Install()
        {
            //if already installed
            if (_installedHandle.Exists())
                return true;

            using (var ioLock = new IoLock(InstallDir, _installingHandleName))
            {
                var lockAcquired = ioLock.TryAcquire();

                //someone else already installed the app in another thread so we're aborting
                if (lockAcquired && IsInstalled())
                    return true;

                //install and don't rely on rollback (try/catch) since it might conflict with other task
                foreach (var installAction in _installActions)
                    installAction();

                //app installed successfully
                File.Create(_installedHandle.FullName())?.Close();
            }

            return IsInstalled();
        }

        public bool IsInstalled()
        {
            return _installedHandle.Exists();
        }

        public bool UnInstall()
        {
            InstallDir.DeleteIfExists();
            return IsInstalled() == false;
        }
    }
}
