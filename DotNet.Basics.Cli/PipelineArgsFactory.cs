﻿using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli
{
    public class PipelineArgsFactory
    {
        public virtual object Create(Type pipelineType, ArgsDictionary args, ILogger? log = null)
        {
            var argsPipelineType = pipelineType;

            while (argsPipelineType!.GetGenericArguments().Any() == false)
                argsPipelineType = argsPipelineType.BaseType;

            var argsType = argsPipelineType.GetGenericArguments().Single();
            var argsObject = Activator.CreateInstance(argsType) ?? throw new ArgumentException($"Failed to instantiate object of type: {argsType.FullName}");
            var writeProperties = argsType.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance)
                .Where(p => p.CanWrite).ToList();

            var unusedList = args.Keys.ToList();
            var mandatoryPropertiesNotSet = new List<(string ArgName, string ArgType)>();

            foreach (var prop in writeProperties)
            {
                if (args.ContainsKey(prop.Name))
                {
                    unusedList.RemoveAt(unusedList.IndexOf(prop.Name.ToLowerInvariant()));

                    var parserType = Nullable.GetUnderlyingType(prop.PropertyType) != null
                        ? Nullable.GetUnderlyingType(prop.PropertyType)
                        : prop.PropertyType;

                    var parser = GetParser(parserType!);
                    try
                    {
                        prop.SetValue(argsObject, parser.Invoke(args[prop.Name]));
                    }
                    catch (FormatException e)
                    {
                        log?.Debug(e.ToString());
                        throw new FormatException($"Failed to parse arg for {prop.Name.Highlight()}. Value was: {args[prop.Name]?.Highlight()}");
                    }
                }
                //assert mandatory properties = nullable and null
                var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (underlyingType != null && prop.GetValue(argsObject) == null)
                    mandatoryPropertiesNotSet.Add((prop.Name, $"{underlyingType.Name}?"));
            }
            if (mandatoryPropertiesNotSet.Any())
                throw new MissingArgumentException(argsType, mandatoryPropertiesNotSet);
            if (unusedList.Any())//all provided arguments must be accepted by args object. Not all Setters must be provided
                throw new UnknownArgumentsException(unusedList);


            return argsObject;
        }

        public virtual Func<string?, object> GetParser(Type argsType)
        {
            if (argsType == null) throw new ArgumentNullException(nameof(argsType));
            return argsType switch
            {
                //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
                not null when argsType == typeof(string) => val => val ?? string.Empty,
                not null when argsType == typeof(bool) => val => string.IsNullOrEmpty(val) || bool.Parse(val),
                not null when argsType == typeof(byte) => val => byte.Parse(val ?? "0"),
                not null when argsType == typeof(sbyte) => val => sbyte.Parse(val ?? "0"),
                not null when argsType == typeof(char) => val => val == null ? '\u0000' : char.Parse(val),
                not null when argsType == typeof(decimal) => val => decimal.Parse(val ?? "0"),
                not null when argsType == typeof(double) => val => double.Parse(val ?? "0"),
                not null when argsType == typeof(float) => val => float.Parse(val ?? "0"),
                not null when argsType == typeof(int) => val => int.Parse(val ?? "0"),
                not null when argsType == typeof(uint) => val => uint.Parse(val ?? "0"),
                not null when argsType == typeof(nint) => val => nint.Parse(val ?? "0"),
                not null when argsType == typeof(nuint) => val => nuint.Parse(val ?? "0"),
                not null when argsType == typeof(long) => val => long.Parse(val ?? "0"),
                not null when argsType == typeof(ulong) => val => ulong.Parse(val ?? "0"),
                not null when argsType == typeof(short) => val => short.Parse(val ?? "0"),
                not null when argsType == typeof(ushort) => val => ushort.Parse(val ?? "0"),
                //dotnet.basics types
                not null when argsType == typeof(DirPath) => val => val?.ToDir() ?? string.Empty,
                not null when argsType == typeof(FilePath) => val => val?.ToFile() ?? string.Empty,
                not null when argsType == typeof(PathInfo) => val => val?.ToPath() ?? string.Empty,
                not null when argsType == typeof(SemVersion) => SemVersion.Parse,

                not null when argsType == typeof(string[]) => val => val?.Split('|').ToArray() ?? [],
                not null when argsType == typeof(DirPath[]) => val => val?.Split('|').Select(v => v.ToDir()).ToArray() ?? [],
                not null when argsType == typeof(FilePath[]) => val => val?.Split('|').Select(v => v.ToFile()).ToArray() ?? [],
                not null when argsType == typeof(PathInfo[]) => val => val?.Split('|').Select(v => v.ToPath()).ToArray() ?? [],
                not null when argsType.IsAssignableTo(typeof(IEnumerable<string>)) => val => val?.Split('|').ToList() ?? [],
                not null when argsType.IsAssignableTo(typeof(IEnumerable<DirPath>)) => val => val?.Split('|').ToList() ?? [],
                not null when argsType.IsAssignableTo(typeof(IEnumerable<FilePath>)) => val => val?.Split('|').ToList() ?? [],
                not null when argsType.IsAssignableTo(typeof(IEnumerable<PathInfo>)) => val => val?.Split('|').ToList() ?? [],
                _ => val => Parse(argsType!, val)
            };
        }

        protected virtual object Parse(Type argsType, string? val)
        {
            throw new NotSupportedException(argsType.FullName);
        }
    }
}


