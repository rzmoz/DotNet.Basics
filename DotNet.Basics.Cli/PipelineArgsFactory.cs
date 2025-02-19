using System;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.Cli
{
    public class PipelineArgsFactory
    {
        public object Create(Type pipelineType, ArgsDictionary args, params IArgParser[] argParsers)
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

        private Func<string, object> GetParser(Type t, params IArgParser[] argParsers)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            return t switch
            {
                //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
                not null when t == typeof(bool) => val => bool.Parse(val),
                not null when t == typeof(byte) => val => byte.Parse(val),
                not null when t == typeof(sbyte) => val => sbyte.Parse(val),
                not null when t == typeof(char) => val => char.Parse(val),
                not null when t == typeof(decimal) => val => decimal.Parse(val),
                not null when t == typeof(double) => val => double.Parse(val),
                not null when t == typeof(float) => val => float.Parse(val),
                not null when t == typeof(int) => val => int.Parse(val),
                not null when t == typeof(uint) => val => uint.Parse(val),
                not null when t == typeof(nint) => val => nint.Parse(val),
                not null when t == typeof(nuint) => val => nuint.Parse(val),
                not null when t == typeof(long) => val => long.Parse(val),
                not null when t == typeof(ulong) => val => ulong.Parse(val),
                not null when t == typeof(short) => val => short.Parse(val),
                not null when t == typeof(ushort) => val => ushort.Parse(val),
                _ => val =>
                {
                    var parser = argParsers.FirstOrDefault(p => p.CanParse(t!));
                    return parser != null ? parser.Parse(val) : throw new NotSupportedException(t!.FullName);
                }
            };
        }
    }
}


