using System;
using DotNet.Basics.Cli;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Tests.Cli
{
    public class TestCliHost : CliHost<TestArgs>
    {
        public TestCliHost(TestArgs args, string[] rawArgs, IConfigurationRoot config, ILogDispatcher log) : base(args, rawArgs, config, log)
        { }
        public string MySetting => base[nameof(MySetting)];
    }
}
