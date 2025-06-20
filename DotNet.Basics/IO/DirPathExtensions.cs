﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Collections;

namespace DotNet.Basics.IO
{
    public static class DirPathExtensions
    {
        public static DirPath Add(this DirPath dp, DirPath subPath)
        {
            return dp.Add(subPath.Segments.ToArray());
        }
        public static DirPath Add(this DirPath dp, params string[] subPath)
        {
            return dp.ToDir(subPath);
        }

        public static bool IsEmpty(this DirPath dp)
        {
            return dp.Exists() && dp.EnumeratePaths().None();
        }

        public static bool CleanIfExists(this DirPath dp, params string[] subFolders)
        {
            return dp.Add(subFolders).CleanIfExists(20.Seconds());
        }

        public static bool CleanIfExists(this DirPath dp, TimeSpan timeout)
        {
            if (dp.Exists() == false)
                return true;

            try
            {
                Parallel.ForEach(dp.EnumeratePaths(), path => { path.DeleteIfExists(timeout); });
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException != null)
                    throw ae.InnerException;
                throw;
            }

            return dp.GetPaths().Count == 0;
        }

        public static DirPath CreateIfNotExists(this DirPath dp, params string[] subFolders)
        {
            var dir = dp.Add(subFolders);
            if (dir.Exists() == false)
            {
                var path = dir.FullName;
                if (!PathInfo.IsWindowsRooted(path))
                {
                    path = path.RemovePrefix(Directory.GetCurrentDirectory());
                    if (path != dir.FullName)
                        path = path.RemovePrefix('/');
                }

                Directory.CreateDirectory(path);
            }

            return dir;
        }

        public static DirPath CreateSubDir(this DirPath dp, string subDirName)
        {
            var subDir = dp.Add(subDirName);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public static void CopyTo(this DirPath dp, DirPath target, bool includeSubfolders = false)
        {
            if (dp.Exists() == false)
                return;

            var targetPath = target.FullName.ToLowerInvariant();
            var sourcePath = dp.FullName.ToLowerInvariant();

            //if copy to self
            if (targetPath == sourcePath)
                return;

            if (targetPath.StartsWith(sourcePath))
                throw new IOException(
                    $"Target path is a sub path of Source path. Target: {targetPath} | Source: {sourcePath}");

            target.CreateIfNotExists();

            if (includeSubfolders)
            {
                Parallel.ForEach(dp.GetDirectories(), dir =>
                {
                    var nextTargetSubDir = target.ToDir(dir.Name);
                    nextTargetSubDir.CreateIfNotExists();
                    dir.CopyTo(nextTargetSubDir, true);
                });
            }

            Parallel.ForEach(dp.EnumerateFiles(),
                file => { file.CopyTo(target, overwrite: true, ensureTargetDir: false); });
        }

        public static IReadOnlyCollection<DirPath> GetDirectories(this DirPath dp, string? searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return dp.EnumerateDirectories(searchPattern ?? "*", searchOption).ToArray();
        }

        public static IReadOnlyCollection<FilePath> GetFiles(this DirPath dp, string? searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return dp.EnumerateFiles(searchPattern ?? "*", searchOption).ToArray();
        }

        public static IReadOnlyCollection<PathInfo> GetPaths(this DirPath dp, string? searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return dp.EnumeratePaths(searchPattern ?? "*", searchOption).ToArray();
        }

        public static IEnumerable<DirPath> EnumerateDirectories(this DirPath dp, string? searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateDirectories(dp.FullName, searchPattern ?? "*", searchOption)
                .Select(dir => dir.ToDir());
        }

        public static IEnumerable<FilePath> EnumerateFiles(this DirPath dp, string? searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateFiles(dp.FullName, searchPattern ?? "*", searchOption)
                .Select(file => file.ToFile());
        }

        public static IEnumerable<PathInfo> EnumeratePaths(this DirPath dp, string? searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return Directory.EnumerateFileSystemEntries(dp.FullName, searchPattern ?? "*", searchOption)
                .Select(fse => fse.ToPath());
        }
    }
}