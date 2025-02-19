using System;
using System.Linq;
using System.Reflection;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

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
            var argsProperties = argsType.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            foreach (var argsProperty in argsProperties)
            {
                if (!argsProperty.CanWrite)
                    continue;

                var parser = GetParser(argsProperty.PropertyType);

                argsProperty.SetValue(argsObject, parser.Invoke(args[argsProperty.Name]));
            }

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
                _ => val =>
                {
                    var parser = argParsers.FirstOrDefault(p => p.CanParse(argsType!));
                    return parser != null ? parser.Parse(val) : throw new NotSupportedException(argsType!.FullName);
                }
            };
        }
    }
}


