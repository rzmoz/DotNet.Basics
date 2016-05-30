using System;
using System.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class TempDir : IDisposable
    {
        public TempDir(string dirPrefix = null)
        {
            Prefix = dirPrefix == null ? string.Empty : dirPrefix.EnsureSuffix(".");
            Dir = System.IO.Path.GetTempPath().ToPath($"{Prefix}{Guid.NewGuid()}").ToDir();
            Dir.CleanIfExists();
            Dir.CreateIfNotExists();
        }

        public string Prefix { get; }
        public DirectoryInfo Dir { get; }
        
        public void Dispose()
        {
            Dir.DeleteIfExists();
        }
    }
}
