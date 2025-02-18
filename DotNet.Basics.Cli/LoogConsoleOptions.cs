using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleOptions(string[] rawArgs, Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>? argsParser)
    {
        private static readonly SysRegex _argRegex = @"^--(?<key>.+?)=(?<value>.+)";
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
        public IReadOnlyList<string> RawArgs => rawArgs.Blacklist(VerboseFlag, ADOFlag, DebugFlag).ToArray();
        public IReadOnlyDictionary<string, string> ParsedArgs { get; } = argsParser?.Invoke(rawArgs) ?? DefaultArgsParse(rawArgs);
        public IReadOnlyDictionary<string, Func<Exception, int>> ExceptionHandlers => _exceptionHandlers;

        public T GetService<T>()
        {
            return Services.GetService<T>() ?? throw new NullReferenceException(typeof(T).FullName);
        }

        public LoogConsoleOptions WithExceptionHandler<T>(Func<T, int> exceptionHandler) where T : Exception
        {
            _exceptionHandlers.Add(typeof(T).Name, e => exceptionHandler.Invoke((T)e));
            return this;
        }

        public static IReadOnlyDictionary<string, string> DefaultArgsParse(IReadOnlyList<string> args)
        {
            return args.Select(a =>
            {
                var match = _argRegex.Regex.Match(a);
                return new KeyValuePair<string, string>(match.Groups["key"].Value.ToLowerInvariant(), match.Groups["value"].Value.Trim('"'));
            }).ToDictionary();
        }


        private bool HasFlag(string flag)
        {
            return rawArgs.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
