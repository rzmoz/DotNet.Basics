using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using System.Collections.Generic;
using System.IO;
using System;

namespace DotNet.Basics.Win
{
    public class FileApplication(DirPath installDir)
    {
        private const string _installingHandleName = "installing.dat";
        private const string _installedHandleName = "installed.dat";
        private readonly FilePath _installedHandle = installDir.ToFile(_installedHandleName)!;
        private readonly IList<Action> _installActions = new List<Action>();

        public FileApplication(string appName)
            : this(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToDir(appName)!)
        { }

        public DirPath InstallDir { get; } = installDir ?? throw new ArgumentNullException(nameof(installDir));

        public int RunFromCmd(string fileName, IEnumerable<string> args, CmdPromptLogger? logger = null)
        {
            return RunFromCmd(fileName, args, logger?.WriteOutput, logger?.WriteError, logger?.WriteDebug);
        }

        public int RunFromCmd(string fileName, IEnumerable<string> args, Action<string>? writeOutput = null, Action<string>? writeError = null, Action<string>? writeDebug = null)
        {
            Install();
            var argString = args.JoinString(" ");

            var file = InstallDir.ToFile(fileName)!;
            if (file.Exists() == false)
                throw new FileNotFoundException(file.FullName);

            return CmdPrompt.Run($"{file.FullName} {argString}", writeOutput, writeError, writeDebug);
        }

        public FileApplication WithStream<T>(string fileName, Action<FilePath>? postInstallAction = null)
        {
            var stream = typeof(T).Assembly.GetManifestResourceStream(typeof(T), fileName) ?? throw new IOException($"Failed to get manifest stream for {fileName} in {typeof(T).FullName}");
            return WithStream(fileName, stream, true, postInstallAction);
        }

        public FileApplication WithStream(string filename, Stream content, Action<FilePath>? postInstallAction = null)
        {
            return WithStream(filename, content, true, postInstallAction);
        }
        public FileApplication WithStream(string filename, Stream content, bool disposeStreamWhenDone, Action<FilePath>? postInstallAction = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var target = InstallDir.ToFile(filename)!;
            _installActions.Add(() =>
            {
                target.DeleteIfExists();//we ensure file integrity if we got this far. No guarantees that corrupt files haven't been left behind by a faulty installation
                using (var fsDst = new FileStream(target.FullName, FileMode.Create, FileAccess.Write))
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
                File.Create(_installedHandle.FullName)?.Close();
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
