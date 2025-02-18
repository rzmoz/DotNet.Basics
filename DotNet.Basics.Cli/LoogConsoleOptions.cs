using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Serilog.Looging;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleOptions(string[] rawArgs, Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>? argsParser)
    {
        private const string _argsDefaultParsePattern = @"^--(?<key>.+?)=(?<value>.+)";
        private static readonly Regex _argsDefaultParseRegex = new Regex(_argsDefaultParsePattern, RegexOptions.Compiled);


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
        public IReadOnlyList<string> RawArgs => RemoveStandardFlags(rawArgs);
        public IReadOnlyDictionary<string, string> ParsedArgs { get; } = argsParser?.Invoke(rawArgs) ?? DefaultArgsParse(rawArgs);

        public T GetService<T>()
        {
            return Services.GetService<T>() ?? throw new NullReferenceException(typeof(T).FullName);
        }
        public T GetService<T>(Type t)
        {
            return (T)Services.GetService(t)!;
        }

        public static IReadOnlyDictionary<string, string> DefaultArgsParse(IReadOnlyList<string> args)
        {
            return args.Select(a =>
            {
                var match = _argsDefaultParseRegex.Match(a);
                return new KeyValuePair<string, string>(match.Groups["key"].Value.ToLowerInvariant(), match.Groups["value"].Value.Trim('"'));
            }).ToDictionary();
        }


        private bool HasFlag(string flag)
        {
            return rawArgs.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }

        private IReadOnlyList<string> RemoveStandardFlags(string[] args)
        {
            return rawArgs.Where(a => !a.Equals(ADOFlag) && !a.Equals(DebugFlag) && !a.Equals(VerboseFlag)).ToArray();
        }
    }
}
