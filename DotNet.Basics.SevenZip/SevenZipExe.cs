using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.SevenZip
{
    public class SevenZipExe
    {
        private static readonly Assembly _sevenZipAssembly = typeof(SevenZipExe).Assembly;

        private readonly CliApplication _sevenZipApp;

        public SevenZipExe(DirPath installDir)
        {
            _sevenZipApp = new CliApplication(installDir.ToDir("7Zip"), "7za.exe", _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7za.exe"))
                .WithFile("7za.dll", _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7za.dll"))
                .WithFile("7zxa.dll", _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7zxa.dll"));
        }

        public (string Input, int ExitCode, string Output) ExtractToDirectory(string archivePath, string targetDirPath)
        {
            if (File.Exists(archivePath) == false)
                throw new IOException($"Archive not found: {archivePath}");
            if (Directory.Exists(targetDirPath))
                throw new IOException($"Target dir already exists at: {targetDirPath}");
            return ExecuteSevenZip("x", $"\"{archivePath}\"", $"\"-o{targetDirPath.ToDir().FullName()}\"", "*", "-r", "aoa");
        }

        public (string Input, int ExitCode, string Output) CreateFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            if (overwrite == false && archivePath.ToFile().Exists())
                throw new IOException($"Target archive path already exists: {archivePath}. Set overwrite to true to ignore");

            archivePath.ToFile().DeleteIfExists();
            return ExecuteSevenZip("a", $"\"{archivePath}\"", $"\"{sourceDirPath.ToDir().FullName()}\\*\"", "-tzip", "-mx3", "-mmt");
        }

        public (string Input, int ExitCode, string Output) ExecuteSevenZip(string command, params string[] @params)
        {
            var allArgs = new List<string>();
            allArgs.Add(command);
            allArgs.AddRange(@params);
            allArgs.Add("-y");
            return _sevenZipApp.Run(allArgs.ToArray());
        }
    }
}
