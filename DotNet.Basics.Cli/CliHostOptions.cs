using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class CliHostOptions(CliHostBuilderOptions builderOptions, IServiceProvider serviceProvider)
    {
        public int FatalExitCode => builderOptions.FatalExitCode;

        public IServiceProvider Services { get; } = serviceProvider;

        public ArgsDictionary Args => builderOptions.Args;
        public bool Verbose => builderOptions.Verbose;
        public bool ADO => builderOptions.ADO;
        public bool Debug => builderOptions.Debug;

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
