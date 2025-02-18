using System;
using System.Collections.Generic;
using DotNet.Basics.Serilog.Looging;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleOptions(string[] args, Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>? argsParser)
    {
        public int FatalExitCode { get; set; } = 500;

        public TimeSpan LongRunningOperationsPingInterval { get; set; } = TimeSpan.FromMinutes(1);
        public ILoog Log => GetService<ILoog>();
        public IServiceProvider Services { get; set; } = new ServiceCollection().BuildServiceProvider();
        public IReadOnlyList<string> RawArgs { get; } = args;

        public ArgsProvider Args => new(args, argsParser);
        public bool Verbose => Args.HasFlag(nameof(Verbose));
        public bool ADO => Args.HasFlag(nameof(ADO));
        public bool Debug => Args.HasFlag(nameof(Debug));

        public T GetService<T>()
        {
            return Services.GetService<T>() ?? throw new NullReferenceException(typeof(T).FullName);
        }
        public T GetService<T>(Type t)
        {
            return (T)Services.GetService(t)!;
        }


    }
}
