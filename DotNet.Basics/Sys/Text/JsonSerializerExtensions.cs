using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.Basics.Sys.Text
{
    public static class JsonSerializerExtensions
    {
        private static JsonSerializerOptions GetJsonSerializerOptions(bool writeIndented) => new JsonSerializerOptions
        {
            WriteIndented = writeIndented,
            Converters =
            {
                new DirPathJsonConverter(),
                new FilePathJsonConverter(),
                new PathInfoJsonConverter(),
                new SemVersionJsonConverter(),
                new JsonStringEnumConverter()
            }
        };

        public static string SerializeToJson<TValue>(this TValue value, bool writeIndented = false, Action<JsonSerializerOptions> updateOptions = null)
        {
            var options = GetJsonSerializerOptions(writeIndented);
            updateOptions?.Invoke(options);
            return JsonSerializer.Serialize(value, options);
        }
        public static TValue DeserializeJson<TValue>(this string json, bool writeIndented = false, Action<JsonSerializerOptions> updateOptions = null)
        {
            var options = GetJsonSerializerOptions(writeIndented);
            updateOptions?.Invoke(options);
            return JsonSerializer.Deserialize<TValue>(json, options);
        }
    }
}
