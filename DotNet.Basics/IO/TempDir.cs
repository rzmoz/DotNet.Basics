using System;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class TempDir : IDisposable
    {

        private static readonly Random _random = new Random();

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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
