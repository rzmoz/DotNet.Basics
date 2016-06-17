﻿using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Compression
{
    public class SevenZip
    {
        private readonly ILogger _logger;

        public SevenZip(ILogger logger = null)
        {
            _logger = logger;
        }

        public int ExtractToDirectory(string archivePath, string targetDirPath)
        {
            return ExecuteSevenZip("x", $"\"{archivePath}\"", $"\"-o{targetDirPath.ToDir().FullName}\"", "*", "-r");
        }

        public int CreateFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            if (overwrite == false && archivePath.ToPath().Exists())
                throw new System.IO.IOException($"Target archive path already exists: {archivePath}. Set overwrite to true to ignore");

            return ExecuteSevenZip("a", $"\"{archivePath}\"", $"\"{sourceDirPath.ToDir().FullName}\\*\"", "-tzip", "-mx3", "-mmt");
        }

        private int ExecuteSevenZip(string command, params string[] @params)
        {
            using (var temp = new TempDir("7z"))
            {
                var temp7ZExe = temp.Root.ToFile("7za.exe");
                Extract(temp7ZExe.FullName, CompressionResources._7za);
                var temp7ZipDll = temp.Root.ToFile("7za.dll");
                Extract(temp7ZipDll.FullName, CompressionResources._7za1);
                var temp7Zip32Dll = temp.Root.ToFile("7zxa.dll");
                Extract(temp7Zip32Dll.FullName, CompressionResources._7zxa);

                var paramsString = @params.Aggregate(string.Empty, (current, param) => current + $" {param}");
                var script = $"{temp7ZExe.FullName} {command} {paramsString} -y";
                return CommandPrompt.Run(script, _logger);
            }
        }

        private void Extract(string path, byte[] bytes)
        {
            using (var fsDst = new System.IO.FileStream(path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
                fsDst.Write(bytes, 0, bytes.Length);
            
        }
    }
}