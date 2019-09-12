using DotNet.Basics.Cli;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Tests.Cli
{
    public class TestCliHost : CliHost
    {
        public TestCliHost(string[] args, IConfigurationRoot config, ILogDispatcher log) : base(args, config, log)
        { }

        public string MySetting => base[nameof(MySetting)];
    }
}
