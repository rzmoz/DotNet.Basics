using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class TempDir : IDisposable
    {
        public TempDir(string dirPrefix = null)
        {
            Prefix = dirPrefix == null ? string.Empty : dirPrefix.EnsureSuffix(".");
            Root = System.IO.Path.GetTempPath().ToPath(DetectOptions.SetToDir, $"{Prefix}{Guid.NewGuid()}");
            Root.CleanIfExists();
            Root.CreateIfNotExists();
        }

        public string Prefix { get; }
        public Path Root { get; }

        public void Dispose()
        {
            Root.DeleteIfExists();
        }
    }
}
