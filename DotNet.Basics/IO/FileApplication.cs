using System;
using System.Collections.Generic;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class FileApplication
    {
        private readonly Action<string> _writeOutput;
        private readonly Action<string> _writeError;
        private readonly Action<string> _writeDebug;
        private const string _installingHandleName = "installing.dat";
        private const string _installedHandleName = "installed.dat";
        private readonly FilePath _installedHandle;
        private readonly IList<Action> _installActions;

        public FileApplication(string appName, Action<string> writeOutput = null, Action<string> writeError = null, Action<string> writeDebug = null)
            : this(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToDir(appName), writeOutput, writeError, writeDebug)
        { }

        public FileApplication(DirPath installDir, Action<string> writeOutput = null, Action<string> writeError = null, Action<string> writeDebug = null)
        {
            _writeOutput = writeOutput;
            _writeError = writeError;
            _writeDebug = writeDebug;
            InstallDir = installDir ?? throw new ArgumentNullException(nameof(installDir));
            _installedHandle = installDir.ToFile(_installedHandleName);
            _installActions = new List<Action>();
        }

        public DirPath InstallDir { get; }

        public int RunFromCmd(string fileName, params string[] args)
        {
            Install();
            var argString = args.JoinString(" ");

            var file = InstallDir.ToFile(fileName);
            if (file.Exists() == false)
                throw new FileNotFoundException(file.FullName());

            return CmdPrompt.Run($"{file.FullName()} {argString}", _writeOutput, _writeError, _writeDebug);
        }

        public FileApplication WithStream(string filename, Stream content, Action<FilePath> postInstallAction = null)
        {
            return WithStream(filename, content, true, postInstallAction);
        }
        public FileApplication WithStream(string filename, Stream content, bool disposeStreamWhenDone, Action<FilePath> postInstallAction = null)
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
            if (postInstallAction != null)
                _installActions.Add(() => postInstallAction?.Invoke(target));
            return this;
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
