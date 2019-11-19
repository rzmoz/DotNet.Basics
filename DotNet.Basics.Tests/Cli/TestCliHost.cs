using DotNet.Basics.Cli;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Tests.Cli
{
    public class TestCliHost : CliHost
    {
        public TestCliHost(string[] rawArgs, IConfigurationRoot config, ILogDispatcher log) : base(rawArgs, config, log)
        { }

        public string MySetting => base[nameof(MySetting)];
    }
}
