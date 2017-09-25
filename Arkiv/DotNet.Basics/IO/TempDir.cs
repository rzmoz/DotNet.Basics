using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class TempDir : IDisposable
    {
        private readonly int _tempDirLength = 16;

        public TempDir(string dirPrefix = null)
            : this(GetTempDir(), dirPrefix)
        { }

        public TempDir(DirPath parent, string dirPrefix = null)
        {
            Prefix = dirPrefix == null ? string.Empty : dirPrefix.EnsureSuffix(".");
            Root = (parent ?? GetTempDir()).Add($"{Prefix}{TempName(_tempDirLength)}");
            Root.CleanIfExists();
            Root.CreateIfNotExists();

            DebugOut.WriteLine($"TempDir created at: {Root}");
        }

        public string Prefix { get; }
        public DirPath Root { get; }

        private static DirPath GetTempDir()
        {
            return System.IO.Path.GetTempPath().ToDir();
        }

        public void Dispose()
        {
            Root.DeleteIfExists();
            DebugOut.WriteLine(Root.Exists() ?
                $"TempDir failed to delete at: {Root}" :
                $"TempDir deleted at: {Root}");
        }

        private string TempName(int length)
        {
            var random = Guid.NewGuid().ToString("N");
            while (random.Length < length)
                random = string.Concat(random, Guid.NewGuid().ToString("N"));
            return random.Substring(0, length);
        }

        public override string ToString()
        {
            return $"{Root?.FullName}";
        }
    }
}
