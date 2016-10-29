using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotNet.Basics.Collections;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.Compression
{
    public class ZipReader : IDisposable
    {
        private readonly Lazy<ZipArchive> _archiveLoader;
        private readonly Lazy<StringKeyDictionary<ZipArchiveEntry>> _entryLoader;
        
        public ZipReader(FilePath path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            Path = path;
            _archiveLoader = new Lazy<ZipArchive>(Open);
            _entryLoader = new Lazy<StringKeyDictionary<ZipArchiveEntry>>(() =>
            {
                var dic = new StringKeyDictionary<ZipArchiveEntry>();
                foreach (var entry in Archive.Entries)
                {
                    dic.Add(entry.FullName, entry);
                }
                return dic;
            });
        }

        public FilePath Path { get; }

        public ZipArchive Archive => _archiveLoader.Value;

        public IReadOnlyCollection<string> EntryNames => _entryLoader.Value.Keys;

        public bool HasEntry(string path)
        {
            return GetEntry(path) != null;
        }

        public string GetEntryContent(string path)
        {
            var entry = GetEntry(path);
            if (entry == null)
                throw new ArgumentException($"Entry not found: {path}");

            if (entry.FullName.EndsWith("/"))//is folder
                throw new ArgumentException($"Entry is found but is a folder. Can't get content from a folder: {path}");

            using (var stream = entry.Open())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return reader.ReadToEnd();
        }

        private ZipArchiveEntry GetEntry(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            path = path.Replace("\\", "/");//added support for backslash

            path = path.TrimStart('/');

            //look for explicit folder match first
            if (path.EndsWith("/"))
                return (from entry in _entryLoader.Value
                       where entry.Key.Equals(path, StringComparison.OrdinalIgnoreCase)
                       select entry.Value).FirstOrDefault();

            //Look for file or folder
            return (from entry in _entryLoader.Value
                    where entry.Key.RemoveSuffix("/").Equals(path,StringComparison.OrdinalIgnoreCase)//we don't distinguish between files nor folders. Files are found before folders
                    select entry.Value).FirstOrDefault();
        }

        private ZipArchive Open()
        {
            ZipArchive archive = null;
            Repeat.Task(() => archive = ZipFile.Open(Path.FullName, ZipArchiveMode.Read))
                .WithOptions(o =>
                    {
                        o.RetryDelay = 1.Seconds();
                        o.MaxTries = 10;
                    })
                    .Until(() => archive != null);
            return archive;
        }

        public void Dispose()
        {
            if (_archiveLoader.IsValueCreated)
                _archiveLoader.Value.Dispose();
        }
    }
}
