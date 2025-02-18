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
        private const string _argsDefaultFlagPattern = @"^--(?<key>[^=]+?)";
        private static readonly Regex _argsDefaultFlagRegex = new Regex(_argsDefaultFlagPattern, RegexOptions.Compiled);
        private const string _argsDefaultFullParsePattern = @"^--(?<key>.+?)=(?<value>.+)";
        private static readonly Regex _argsDefaultFullParseRegex = new Regex(_argsDefaultFullParsePattern, RegexOptions.Compiled);

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
        public IReadOnlyList<string> RawArgs => rawArgs;
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
                string key;
                var value = string.Empty;

                if (_argsDefaultFullParseRegex.IsMatch(a))
                {
                    var match = _argsDefaultFullParseRegex.Match(a);
                    key = match.Groups["key"].Value;
                    value = match.Groups["value"].Value.Trim('"');
                }
                else if (_argsDefaultFlagRegex.IsMatch(a))
                {
                    var match = _argsDefaultFlagRegex.Match(a);
                    key = match.Groups["key"].Value.ToLowerInvariant();
                }
                else
                    key = a;

                return new KeyValuePair<string, string>(key.ToLowerInvariant(), value);

            }).Where(kvp => !string.IsNullOrEmpty(kvp.Key)).ToDictionary();
        }


        private bool HasFlag(string flag)
        {
            return rawArgs.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
