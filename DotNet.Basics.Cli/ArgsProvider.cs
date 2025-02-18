using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Cli
{
    public class ArgsProvider(IReadOnlyList<string> args, Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>? argsParser)
    {
        private const string _argsDefaultFlagPattern = @"^--(?<key>[^=]+)";
        private static readonly Regex _argsDefaultFlagRegex = new(_argsDefaultFlagPattern, RegexOptions.Compiled);
        private const string _argsDefaultFullParsePattern = @"^--(?<key>.+?)=(?<value>.+)";
        private static readonly Regex _argsDefaultFullParseRegex = new(_argsDefaultFullParsePattern, RegexOptions.Compiled);

        private readonly IReadOnlyDictionary<string, string> _argsDictionary = argsParser?.Invoke(args) ?? DefaultArgsParse(args);

        public string this[string key]
        {
            get
            {
                try
                {
                    return _argsDictionary[key.ToLowerInvariant()];
                }
                catch (KeyNotFoundException)
                {
                    throw new CliArgNotFoundException(key);
                }
            }
        }

        public bool HasFlag(string name)
        {
            return _argsDictionary.ContainsKey(name.ToLowerInvariant().TrimStart('-'));
        }

        public IReadOnlyList<string> RawArgs => args;

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
    }
}
