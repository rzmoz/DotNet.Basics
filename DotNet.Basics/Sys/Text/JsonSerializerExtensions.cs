using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.Basics.Sys.Text
{
    public static class JsonSerializerExtensions
    {
        private static JsonSerializerOptions GetJsonSerializerOptions(bool writeIndented, JsonNamingPolicy jsonNamingPolicy) => new()
        {
            WriteIndented = writeIndented,
            PropertyNamingPolicy = jsonNamingPolicy,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new DirPathJsonConverter(),
                new FilePathJsonConverter(),
                new PathInfoJsonConverter(),
                new SemVersionJsonConverter(),
                new JsonStringEnumConverter()
            }
        };

        public static string ToJson<TValue>(this TValue value, Action<JsonSerializerOptions> options = null)
        {
            return ToJson(value, false, options);
        }
        public static string ToJson<TValue>(this TValue value, bool writeIndented, Action<JsonSerializerOptions> options = null)
        {
            return ToJson(value, writeIndented, JsonNamingPolicy.CamelCase, options);
        }
        public static string ToJson<TValue>(this TValue value, bool writeIndented, JsonNamingPolicy jsonNamingPolicy, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(writeIndented, jsonNamingPolicy);
            options?.Invoke(defOptions);
            return JsonSerializer.Serialize(value, defOptions);
        }
        public static object FromJson(this string json, Type returnType, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(false, JsonNamingPolicy.CamelCase);
            options?.Invoke(defOptions);
            return JsonSerializer.Deserialize(json, returnType, defOptions);
        }
        public static TValue FromJson<TValue>(this string json, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(false, JsonNamingPolicy.CamelCase);
            options?.Invoke(defOptions);
            return JsonSerializer.Deserialize<TValue>(json, defOptions);
        }
        public static TValue FromJsonStream<TValue>(this Stream jsonStream, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(false, JsonNamingPolicy.CamelCase);
            options?.Invoke(defOptions);
            return JsonSerializer.Deserialize<TValue>(jsonStream, defOptions);
        }
    }
}
