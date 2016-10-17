using System;
using System.Collections.Generic;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public class ApplicationInstaller : IDisposable
    {
        private const string _installingHandleName = "installing.dat";
        private const string _installedHandleName = "installed.dat";
        private readonly FilePath _installedHandle;
        private readonly IList<Action> _installActions;

        public ApplicationInstaller(DirPath installDir, string executableFilename)
        {
            if (installDir == null) throw new ArgumentNullException(nameof(installDir));
            InstallDir = installDir;
            Executable = installDir.ToFile(executableFilename);
            _installedHandle = installDir.ToFile(_installedHandleName);
            _installActions = new List<Action>();
        }

        public ApplicationInstaller(DirPath applicationsRoot, string applicationName, string executableFilename)
            : this(applicationsRoot.ToDir(applicationName), executableFilename)
        {
            if (applicationsRoot == null) throw new ArgumentNullException(nameof(applicationsRoot));
            if (applicationName == null) throw new ArgumentNullException(nameof(applicationName));
        }

        public DirPath InstallDir { get; }
        public FilePath Executable { get; }

        public void AddFromBytes(string filename, byte[] content)
        {
            var target = InstallDir.ToFile(filename);
            _installActions.Add(() =>
            {
                target.DeleteIfExists();//we ensure file integrity if we got this far. No guarantess that corrupt files haven't been left behind by a faulty installation
                using (var fsDst = new FileStream(target.FullName, FileMode.Create, FileAccess.Write))
                    fsDst.Write(content, 0, content.Length);
            });
        }

        public void Install()
        {
            //if already installed
            if (_installedHandle.Exists())
                return;

            InstallDir.CreateIfNotExists();

            FileStream installingHandle = null;

            //try to acquire exclusive installing ownership
            try
            {
                //try get install handle
                var handleAcquired = Repeat.Task(() =>
                  {
                      installingHandle = File.Create(InstallDir.ToFile(_installingHandleName).FullName, 128, FileOptions.DeleteOnClose);
                  }).WithOptions(o =>
                  {
                      o.MaxTries = 10;
                      o.RetryDelay = 1.Seconds();
                      o.DontRethrowOnTaskFailedType = typeof(IOException);
                  }).UntilNoExceptions();

                //someone else already installed the app in another thread so we're aborting
                if (handleAcquired && IsInstalled())
                    return;

                //install and don't rely on rollback (try/catch) since it might conflict with other task
                foreach (var installAction in _installActions)
                    installAction();

                //app installed succesfully
                File.Create(_installedHandle.FullName);
            }
            finally
            {
                installingHandle?.Dispose();
                installingHandle = null;
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

        public void Dispose()
        {
            Install();
        }
    }
}
