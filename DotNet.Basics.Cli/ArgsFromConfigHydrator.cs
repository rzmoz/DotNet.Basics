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
    public class ArgsFromConfigHydrator<T> : IArgsHydrator<T>
    {
        private readonly char[] _splitListChar;

        public ArgsFromConfigHydrator(params char[] splitListChar)
        {
            _splitListChar = splitListChar.Length > 0 ? splitListChar : new[] { '|' };
        }

        public virtual T Hydrate(ICliConfiguration config, T args, ILogDispatcher log = null)
        {
            return AutoHydrateFromConfig(config, args, log ?? LogDispatcher.NullLogger);
        }

        protected virtual T AutoHydrateFromConfig(ICliConfiguration config, T args, ILogDispatcher log)
        {
            args.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).ForEachParallel(p =>
                {
                    SetValueFromConfig(config, args, p, log);
                });
            return args;
        }

        protected virtual void SetValueFromConfig(ICliConfiguration config, T args, PropertyInfo p, ILogDispatcher log)
        {
            Func<string, object> valueConversion = null;


            if (p.PropertyType == typeof(DirPath))
                valueConversion = input => input?.ToDir();
            else if (p.PropertyType == typeof(FilePath))
                valueConversion = input => input?.ToFile();
            else if (p.PropertyType == typeof(SemVersion))
                valueConversion = input => input.ToSemVersion();
            else if (p.PropertyType == typeof(bool))
                valueConversion = input =>
                {
                    if (input == null)
                        return false;
                    return input.Equals(ArgsExtensions.IsSetValue) ||
                           input.Equals("true", StringComparison.OrdinalIgnoreCase);
                };
            else if (p.PropertyType == typeof(IReadOnlyList<string>) || p.PropertyType == typeof(IList<string>) || p.PropertyType == typeof(List<string>))
                valueConversion = input => input?.Split(_splitListChar, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            else if (p.PropertyType == typeof(IReadOnlyList<DirPath>))
                valueConversion = input => input?.Split(_splitListChar, StringSplitOptions.RemoveEmptyEntries).Select(path => path.ToDir()).ToArray() ?? new DirPath[0];
            else if (p.PropertyType == typeof(IReadOnlyList<FilePath>))
                valueConversion = input => input?.Split(_splitListChar, StringSplitOptions.RemoveEmptyEntries).Select(path => path.ToFile()).ToArray() ?? new FilePath[0];

            else if (p.PropertyType.IsEnum)
                valueConversion = input => Enum.Parse(p.PropertyType, input, true);
            else if (p.PropertyType == typeof(string))
                valueConversion = null;//no change
            else
                log.Verbose($"Type not first class supported: {p.PropertyType.FullName}");


            SetFromConfig(config, args, p.Name, valueConversion);
        }

        protected virtual void SetFromConfig(ICliConfiguration config, object args, string propertyName, Func<string, object> valueConversion = null)
        {
            if (valueConversion == null)
                valueConversion = input => input;

            var p = args.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (p == null)
                throw new CliException($"Public property: {propertyName} not found on {args.GetType().FullName}");

            if (p.PropertyType == typeof(bool) && config.IsSet(propertyName) || config.HasValue(propertyName))
            {
                p.SetValue(args, valueConversion.Invoke(config[propertyName]));
            }
        }
    }
}
