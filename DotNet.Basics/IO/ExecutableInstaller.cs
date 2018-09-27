using System;
using System.Collections.Generic;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class ExecutableInstaller
    {
        private const string _installingHandleName = "installing.dat";
        private const string _installedHandleName = "installed.dat";
        private readonly FilePath _installedHandle;
        private readonly IList<Action> _installActions;

        public ExecutableInstaller(DirPath installDir, string executableFilename)
        {
            InstallDir = installDir ?? throw new ArgumentNullException(nameof(installDir));
            EntryFile = installDir.ToFile(executableFilename);
            _installedHandle = installDir.ToFile(_installedHandleName);
            _installActions = new List<Action>();
        }

        public ExecutableInstaller(DirPath applicationsRoot, string applicationName, string executableFilename)
            : this(applicationsRoot.ToDir(applicationName), executableFilename)
        {
            if (applicationsRoot == null) throw new ArgumentNullException(nameof(applicationsRoot));
            if (applicationName == null) throw new ArgumentNullException(nameof(applicationName));
        }

        public DirPath InstallDir { get; }
        public FilePath EntryFile { get; }

        public void AddFromStream(string filename, Stream content, bool disposeStreamWhenDone = true)
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
        }

        public void Install()
        {
            //if already installed
            if (_installedHandle.Exists())
                return;

            using (var ioLock = new IoLock(InstallDir, _installingHandleName))
            {
                var lockAcquired = ioLock.TryAcquire();

                //someone else already installed the app in another thread so we're aborting
                if (lockAcquired && IsInstalled())
                    return;
                
                //install and don't rely on rollback (try/catch) since it might conflict with other task
                foreach (var installAction in _installActions)
                    installAction();

                //app installed successfully
                File.Create(_installedHandle.FullName())?.Close();
            }
        }

        public bool IsInstalled()
        {
            return _installedHandle.Exists();
        }

        public void UnInstall()
        {
            InstallDir.DeleteIfExists();
        }
    }
}
