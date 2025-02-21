using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.Basics.IO;

namespace DotNet.Basics.Sys.Text
{
    public class FilePathJsonConverter : JsonConverter<FilePath>
    {
        public override FilePath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString()?.ToFile() ?? throw new ArgumentException($"Unable to get value from {nameof(reader)}"); ;
        }

        public override void Write(Utf8JsonWriter writer, FilePath value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.RawPath);
        }
    }
}
