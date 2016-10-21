using System;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Compression
{
    public class SevenZip
    {
        private readonly DirPath _appRootDir;

        public SevenZip(DirPath appRootDir = null)
        {
            _appRootDir = appRootDir ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToDir();
        }

        public int ExtractToDirectory(string archivePath, string targetDirPath)
        {
            return ExecuteSevenZip("x", $"\"{archivePath}\"", $"\"-o{targetDirPath.ToDir().FullName}\"", "*", "-r", "aoa");
        }

        public int CreateFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            if (overwrite == false && archivePath.ToPath().Exists())
                throw new System.IO.IOException($"Target archive path already exists: {archivePath}. Set overwrite to true to ignore");

            archivePath.ToPath().DeleteIfExists();
            return ExecuteSevenZip("a", $"\"{archivePath}\"", $"\"{sourceDirPath.ToDir().FullName}\\*\"", "-tzip", "-mx3", "-mmt");
        }

        private int ExecuteSevenZip(string command, params string[] @params)
        {
            var filename = InstallsevenZip();
            var paramsString = @params.Aggregate(string.Empty, (current, param) => current + $" {param}");
            var script = $"{filename} {command} {paramsString} -y";
            return CommandPrompt.Run(script);
        }

        private string InstallsevenZip()
        {
            var appInstaller = new ApplicationInstaller(_appRootDir.ToDir("SevenZip"), "7za.exe");
            appInstaller.AddFromBytes(appInstaller.EntryFile.Name, CompressionResources._7za);
            appInstaller.AddFromBytes("7za.dll", CompressionResources._7za1);
            appInstaller.AddFromBytes("7zxa.dll", CompressionResources._7zxa);
            appInstaller.Install();
            return appInstaller.EntryFile.FullName;
        }
    }
}

