using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleOptions(string[] args)
    {
        private readonly Dictionary<string, Func<Exception, int>> _exceptionHandlers = new();

        public string DebugFlag { get; set; } = "--debug";
        public string VerboseFlag { get; set; } = "--verbose";
        public string ADOFlag { get; set; } = "--ADO";

        public int FatalExitCode { get; set; } = 500;
        public bool Verbose => HasFlag(VerboseFlag);
        public bool ADO => HasFlag(ADOFlag);
        public bool Debug => HasFlag(DebugFlag);
        public TimeSpan LongRunningOperationsPingInterval { get; set; } = 1.Minutes();
        public IServiceProvider Services { get; set; } = new ServiceCollection().BuildServiceProvider();
        public IReadOnlyList<string> Args => args.Blacklist(VerboseFlag, ADOFlag, DebugFlag).ToArray();
        public IReadOnlyDictionary<string, Func<Exception, int>> ExceptionHandlers => _exceptionHandlers;
        
        public LoogConsoleOptions WithExceptionHandler<T>(Func<T, int> exceptionHandler) where T : Exception
        {
            _exceptionHandlers.Add(typeof(T).Name, e => exceptionHandler.Invoke((T)e));
            return this;
        }

        

        private bool HasFlag(string flag)
        {
            return args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
