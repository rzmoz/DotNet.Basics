﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Sys
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string enumValue, bool ignoreCase = true) where T : struct
        {
            if (!typeof(T).IsEnum) throw new NotSupportedException($"{typeof(T).FullName} is not an Enum");
            return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
        }

        public static T ToEnum<T>(this string enumValue, T defaultValueIfNotParsed, bool ignoreCase = true) where T : struct
        {
            if (!typeof(T).IsEnum) throw new NotSupportedException($"{typeof(T).FullName} is not an Enum");
            try
            {
                return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
            }
            catch (Exception)
            {
                return defaultValueIfNotParsed;
            }
        }

        public static bool IsEnum<T>(this string enumValue) where T : struct
        {
            try
            {
                enumValue.ToEnum<T>();
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try { return (((int)(object)type & (int)(object)value!) == (int)(object)value); }
            catch { return false; }
        }

        public static IEnumerable<T> GetEnums<T>(this Type @enum) where T : struct
        {
            return @enum.GetEnumNames().Select(e => e.ToEnum<T>());
        }

        public static string ToName(this Enum @enum)
        {
            return Enum.GetName(@enum.GetType(), @enum)!;
        }
    }
}
