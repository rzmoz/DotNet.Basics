using System;
using System.Linq;

namespace DotNet.Basics.Sys
{
    public static class EnumExtensions
    {
        private static readonly int[] _properFlagsValueSequence = new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 };

        public static T ToEnum<T>(this string enumValue, bool ignoreCase = true) where T : struct
        {
            if (!typeof(T).IsEnum) throw new NotSupportedException();
            return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
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

        public static string ToName(this Enum @enum)
        {
            return Enum.GetName(@enum.GetType(), @enum);
        }

        public static bool IsProperFlagsEnum(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.IsEnum == false)
                return false;

            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Any() == false)
                return false;

            var values = Enum.GetValues(type);

            for (var i = 0; i < values.Length; i++)
            {
                var asInt = (int)values.GetValue(i);
                //check that it has value that's exponential related to it's position 
                if (asInt != _properFlagsValueSequence[i])
                    return false;
            }
            return true;
        }

        public static T ToFlagsEnum<T>(this string enumValue) where T : struct
        {
            if (!typeof(T).IsEnum) throw new NotSupportedException();
            if (string.IsNullOrWhiteSpace(enumValue)) throw new ArgumentException("input is null, empty or only white spaces", "enumValue");

            var splitEnumValue = enumValue.Split('|');
            var discreteEnums = splitEnumValue.Select(val => ToEnum<T>(val)).ToList();
            if (discreteEnums.Any() == false)
                throw new FormatException("failed to parse input to:" + typeof(T));

            var @enum = discreteEnums.First();

            //if we have a multiple enums, then we check enum
            if (typeof(T).IsProperFlagsEnum() == false)
                throw new TypeLoadException("Type is not a proper enum. Must have flags attribute set and values must be 1,2,4,8 etc.");

            foreach (var discreteEnum in discreteEnums.Skip(1))
            {
                @enum = (T)(object)(((int)(object)@enum | (int)(object)discreteEnum));
            }
            return @enum;
        }


        public static bool Has<T>(this Enum type, T value)
        {
            try { return (((int)(object)type & (int)(object)value) == (int)(object)value); }
            catch { return false; }
        }


        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }


        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

    }
}
