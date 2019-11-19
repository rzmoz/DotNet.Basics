using System.Collections.Generic;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.Cli
{
    public class TestArgs
    {
        public string Key { get; set; }
        public bool Boolean { get; set; }
        public IReadOnlyList<string> StringList { get; set; }
        public LogLevel Enum{ get; set; }
        public DirPath DirPath { get; set; }
        public FilePath FilePath { get; set; }
    }
}
