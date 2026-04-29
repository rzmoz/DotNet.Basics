using Humanizer;
using System;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilderOptions(ArgsDictionary args)
    {
        public int FatalExitCode { get; set; } = 500;
        public ArgsDictionary Args { get; } = args;
        public bool WithDevColoredConsole { get; set; } = true;
        public TimeSpan LongRunningOperationsPingInterval { get; set; } = 59.Seconds();
    }
}
