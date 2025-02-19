using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;

namespace DotNet.Basics.Cli
{
    public class PipelineArgsFactory
    {
        public virtual object Create(Type pipelineType, ArgsDictionary args, params IArgParser[] argParsers)
        {
            var argsPipelineType = pipelineType;

            while (argsPipelineType!.GetGenericArguments().Any() == false)
                argsPipelineType = argsPipelineType.BaseType;

            var argsType = argsPipelineType.GetGenericArguments().Single();
            var argsObject = Activator.CreateInstance(argsType)!;
            var writeProperties = argsType.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance)
                .Where(p => p.CanWrite).ToList();

            var unusedList = args.Keys.ToList();
            var mandatoryPropertiesNotSet = new List<(string ArgName, string ArgType)>();

            foreach (var prop in writeProperties)
            {
                if (args.ContainsKey(prop.Name))
                {
                    unusedList.RemoveAt(unusedList.IndexOf(prop.Name.ToLowerInvariant()));
                    var parser = GetParser(prop.PropertyType);
                    prop.SetValue(argsObject, parser.Invoke(args[prop.Name]));
                }
                //assert mandatory properties = nullable and null
                var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (underlyingType != null && prop.GetValue(argsObject) == null)
                    mandatoryPropertiesNotSet.Add((prop.Name, $"{underlyingType.Name}?"));
            }
            if (mandatoryPropertiesNotSet.Any())
                throw new MissingArgumentException(argsType, mandatoryPropertiesNotSet);
            if (unusedList.Any())//all provided arguments must be accepted by args object. Not all Setters must be provided
                throw new ArgumentException($"Unknown arguments: {unusedList.ToJson()}");


            return argsObject;
        }

        public virtual Func<string, object> GetParser(Type argsType, params IArgParser[] argParsers)
        {
            if (argsType == null) throw new ArgumentNullException(nameof(argsType));
            return argsType switch
            {
                //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
                not null when argsType == typeof(string) => val => val,
                not null when argsType == typeof(bool) => val => bool.Parse(val),
                not null when argsType == typeof(byte) => val => byte.Parse(val),
                not null when argsType == typeof(sbyte) => val => sbyte.Parse(val),
                not null when argsType == typeof(char) => val => char.Parse(val),
                not null when argsType == typeof(decimal) => val => decimal.Parse(val),
                not null when argsType == typeof(double) => val => double.Parse(val),
                not null when argsType == typeof(float) => val => float.Parse(val),
                not null when argsType == typeof(int) => val => int.Parse(val),
                not null when argsType == typeof(uint) => val => uint.Parse(val),
                not null when argsType == typeof(nint) => val => nint.Parse(val),
                not null when argsType == typeof(nuint) => val => nuint.Parse(val),
                not null when argsType == typeof(long) => val => long.Parse(val),
                not null when argsType == typeof(ulong) => val => ulong.Parse(val),
                not null when argsType == typeof(short) => val => short.Parse(val),
                not null when argsType == typeof(ushort) => val => ushort.Parse(val),
                //dotnet.basics types
                not null when argsType == typeof(DirPath) => val => val.ToDir(),
                not null when argsType == typeof(FilePath) => val => val.ToFile(),
                not null when argsType == typeof(PathInfo) => val => val.ToPath(),
                not null when argsType == typeof(SemVersion) => SemVersion.Parse,

                not null when argsType == typeof(string[]) => val => val.Split('|').ToArray(),
                not null when argsType == typeof(DirPath[]) => val => val.Split('|').Select(v => v.ToDir()).ToArray(),
                not null when argsType == typeof(FilePath[]) => val => val.Split('|').Select(v => v.ToFile()).ToArray(),
                not null when argsType == typeof(PathInfo[]) => val => val.Split('|').Select(v => v.ToPath()).ToArray(),
                not null when argsType.IsAssignableTo(typeof(IEnumerable<string>)) => val => val.Split('|').ToList(),
                not null when argsType.IsAssignableTo(typeof(IEnumerable<DirPath>)) => val => val.Split('|').ToList(),
                not null when argsType.IsAssignableTo(typeof(IEnumerable<FilePath>)) => val => val.Split('|').ToList(),
                not null when argsType.IsAssignableTo(typeof(IEnumerable<PathInfo>)) => val => val.Split('|').ToList(),

                _ => val =>
                {
                    var parser = argParsers.FirstOrDefault(p => p.CanParse(argsType!));
                    return parser != null ? parser.Parse(val) : throw new NotSupportedException(argsType!.FullName);
                }
            };
        }
    }
}


