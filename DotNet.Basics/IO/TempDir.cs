using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class TempDir : IDisposable
    {
        private int _tempDirLength = 16;

        public TempDir(string dirPrefix = null)
            : this(System.IO.Path.GetTempPath().ToDir(), dirPrefix)
        {
        }

        public TempDir(DirPath parent, string dirPrefix = null)
        {
            Prefix = dirPrefix == null ? string.Empty : dirPrefix.EnsureSuffix(".");
            Root = parent.Add($"{Prefix}{TempName(_tempDirLength)}");
            Root.CleanIfExists();
            Root.CreateIfNotExists();
        }

        public string Prefix { get; }
        public DirPath Root { get; }

        public void Dispose()
        {
            Root.DeleteIfExists();
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
