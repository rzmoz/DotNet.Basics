﻿using System;
using System.Collections.Generic;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.SevenZip
{
    public class SevenZipExe
    {
        private readonly Action<string> _writeOutput;
        private readonly Action<string> _writeError;
        private readonly Action<string> _writeDebug;

        private readonly FileApplication _sevenZipApp;
        private const string _entryFileName = "7za.exe";

        public SevenZipExe(Action<string> writeOutput = null, Action<string> writeError = null, Action<string> writeDebug = null)
        {
            _writeOutput = writeOutput;
            _writeError = writeError;
            _writeDebug = writeDebug;
            _sevenZipApp = new FileApplication("7Zip")
                .WithStream<SevenZipExe>(_entryFileName)
                .WithStream<SevenZipExe>("7za.dll")
                .WithStream<SevenZipExe>("7zxa.dll");
        }

        public int ExtractToDirectory(string archivePath, string targetDirPath)
        {
            if (File.Exists(archivePath) == false)
                throw new IOException($"Archive not found: {archivePath}");
            if (Directory.Exists(targetDirPath))
                throw new IOException($"Target dir already exists at: {targetDirPath}");
            return SevenZipCmd("x", $"\"{archivePath}\"", $"\"-o{targetDirPath.ToDir().FullName}\"", "*", "-r", "aoa");
        }

        public int CreateZipFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            return CreateFromDirectory(sourceDirPath, archivePath.TrimEnd('\\').EnsureSuffix(".zip"), "zip", overwrite);
        }

        public int Create7zFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            return CreateFromDirectory(sourceDirPath, archivePath.TrimEnd('\\').EnsureSuffix(".7z"), "7z", overwrite);
        }

        private int CreateFromDirectory(string sourceDirPath, string archivePath, string archiveType, bool overwrite = false)
        {
            if (Directory.Exists(sourceDirPath) == false)
                throw new DirectoryNotFoundException(sourceDirPath);
            if (overwrite)
                archivePath.ToFile().DeleteIfExists();
            else if (archivePath.ToFile().Exists())
                throw new IOException($"Target archive path already exists: {archivePath}. Set overwrite to true to ignore");
            return SevenZipCmd("a", $"\"{archivePath}\"", $"\"{sourceDirPath.ToDir().FullName}\\*\"", $"-t{archiveType}", $"-mx7", "-mmt");
        }

        public int SevenZipCmd(string command, params string[] switches)
        {
            var allArgs = new List<string>
            {
                command
            };
            allArgs.AddRange(switches);
            allArgs.Add("-y");
            return _sevenZipApp.RunFromCmd(_entryFileName, allArgs.ToArray(), _writeOutput, _writeError, _writeDebug);
        }
    }
}
