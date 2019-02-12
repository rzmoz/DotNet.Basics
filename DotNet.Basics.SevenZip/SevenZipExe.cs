using System;
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

        private readonly FileApplication _sevenZipApp;
        private const string _entryFileName = "7za.exe";

        public SevenZipExe(DirPath installDir, Action<string> writeOutput = null, Action<string> writeError = null)
        {
            _sevenZipApp = new FileApplication(installDir.ToDir("7Zip"), writeOutput, writeError)
                .WithStream(_entryFileName, _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7za.exe"))
                .WithStream("7za.dll", _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7za.dll"))
                .WithStream("7zxa.dll", _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7zxa.dll"));
        }

        public (string Input, int ExitCode) ExtractToDirectory(string archivePath, string targetDirPath)
        {
            if (File.Exists(archivePath) == false)
                throw new IOException($"Archive not found: {archivePath}");
            if (Directory.Exists(targetDirPath))
                throw new IOException($"Target dir already exists at: {targetDirPath}");
            return ExecuteSevenZip("x", $"\"{archivePath}\"", $"\"-o{targetDirPath.ToDir().FullName()}\"", "*", "-r", "aoa");
        }

        public (string Input, int ExitCode) CreateFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            if (overwrite == false && archivePath.ToFile().Exists())
                throw new IOException($"Target archive path already exists: {archivePath}. Set overwrite to true to ignore");

            archivePath.ToFile().DeleteIfExists();
            return ExecuteSevenZip("a", $"\"{archivePath}\"", $"\"{sourceDirPath.ToDir().FullName()}\\*\"", "-tzip", "-mx3", "-mmt");
        }

        public (string Input, int ExitCode) ExecuteSevenZip(string command, params string[] @params)
        {
            var allArgs = new List<string>();
            allArgs.Add(command);
            allArgs.AddRange(@params);
            allArgs.Add("-y");
            return _sevenZipApp.RunFromCmd(_entryFileName, allArgs.ToArray());
        }
    }
}
