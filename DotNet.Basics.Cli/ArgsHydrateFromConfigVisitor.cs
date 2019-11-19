using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public class ArgsHydrateFromConfigVisitor
    {
        protected ICliConfiguration Config { get; }
        protected ILogDispatcher Log { get; }
        private readonly char[] _pipeSplit;

        public ArgsHydrateFromConfigVisitor(ICliConfiguration config, ILogDispatcher log, params char[] splitListsChar)
        {
            Config = config;
            Log = log;
            _pipeSplit = splitListsChar.Length > 0 ? splitListsChar : new[] { '|' };
        }
        public virtual T HydrateFromConfig<T>(T args)
        {
            args.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).ForEachParallel(p =>
                {
                    SetValueFromConfig(args, p);
                });
            return args;
        }
        protected virtual void SetValueFromConfig<T>(T args, PropertyInfo p)
        {
            if (p.PropertyType == typeof(DirPath))
                SetFromConfig(args, p.Name, input => input?.ToDir());
            else if (p.PropertyType == typeof(FilePath))
                SetFromConfig(args, p.Name, input => input?.ToFile());
            else if (p.PropertyType == typeof(SemVersion))
                SetFromConfig(args, p.Name, input => input.ToSemVersion());
            else if (p.PropertyType == typeof(bool))
                SetFromConfig(args, p.Name, input =>
                {
                    if (input == null)
                        return false;
                    return input.Equals(ArgsExtensions.IsSetValue) ||
                           input.Equals("true", StringComparison.OrdinalIgnoreCase);
                });
            else if (p.PropertyType == typeof(IReadOnlyList<string>) || p.PropertyType == typeof(IList<string>) || p.PropertyType == typeof(List<string>))
                SetFromConfig(args, p.Name, input => input?.Split(_pipeSplit, StringSplitOptions.RemoveEmptyEntries) ?? new string[0]);
            else if (p.PropertyType == typeof(IReadOnlyList<DirPath>))
                SetFromConfig(args, p.Name, input => input?.Split(_pipeSplit, StringSplitOptions.RemoveEmptyEntries).Select(path => path.ToDir()).ToArray() ?? new DirPath[0]);
            else if (p.PropertyType == typeof(IReadOnlyList<FilePath>))
                SetFromConfig(args, p.Name, input => input?.Split(_pipeSplit, StringSplitOptions.RemoveEmptyEntries).Select(path => path.ToFile()).ToArray() ?? new FilePath[0]);

            else if (p.PropertyType.IsEnum)
                SetFromConfig(args, p.Name, input => Enum.Parse(p.PropertyType, input, true));
            else if (p.PropertyType == typeof(string))
                SetFromConfig(args, p.Name);
            else
            {
                Log.Verbose($"Type not first class supported: {p.PropertyType.FullName}");
                SetFromConfig(args, p.Name);
            }
        }

        protected virtual void SetFromConfig(object args, string propertyName, Func<string, object> valueConversion = null)
        {
            if (valueConversion == null)
                valueConversion = input => input;

            var p = args.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (p == null)
                throw new CliException($"Public property: {propertyName} not found on {args.GetType().FullName}");

            if (p.PropertyType == typeof(bool) && Config.IsSet(propertyName) || Config.HasValue(propertyName))
            {
                p.SetValue(args, valueConversion.Invoke(Config[propertyName]));
            }
        }
    }
}
