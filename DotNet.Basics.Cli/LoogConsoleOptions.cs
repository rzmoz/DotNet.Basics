using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Serilog.Looging;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleOptions(string[] args, Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>? argsParser)
    {
        public string DebugFlag { get; set; } = "--debug";
        public string VerboseFlag { get; set; } = "--verbose";
        public string ADOFlag { get; set; } = "--ADO";

        public int FatalExitCode { get; set; } = 500;
        public bool Verbose => HasFlag(VerboseFlag);
        public bool ADO => HasFlag(ADOFlag);
        public bool Debug => HasFlag(DebugFlag);
        public TimeSpan LongRunningOperationsPingInterval { get; set; } = TimeSpan.FromMinutes(1);
        public ILoog Log => GetService<ILoog>();
        public IServiceProvider Services { get; set; } = new ServiceCollection().BuildServiceProvider();
        public ArgsProvider Args { get; } = new(args,argsParser);

        public T GetService<T>()
        {
            return Services.GetService<T>() ?? throw new NullReferenceException(typeof(T).FullName);
        }
        public T GetService<T>(Type t)
        {
            return (T)Services.GetService(t)!;
        }

        private bool HasFlag(string flag)
        {
            return args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
