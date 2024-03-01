﻿using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

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
            },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string ToJson<TValue>(this TValue value, Action<JsonSerializerOptions> options = null)
        {
            return ToJson(value, false, options);
        }
        public static string ToJson<TValue>(this TValue value, bool writeIndented, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(writeIndented);
            options?.Invoke(defOptions);
            return JsonSerializer.Serialize(value, defOptions);
        }
        public static TValue FromJson<TValue>(this string json, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(false);
            options?.Invoke(defOptions);
            return JsonSerializer.Deserialize<TValue>(json, defOptions);
        }
        public static TValue FromJsonStream<TValue>(this Stream jsonStream, Action<JsonSerializerOptions> options = null)
        {
            var defOptions = GetJsonSerializerOptions(false);
            options?.Invoke(defOptions);
            return JsonSerializer.Deserialize<TValue>(jsonStream, defOptions);
        }
    }
}
