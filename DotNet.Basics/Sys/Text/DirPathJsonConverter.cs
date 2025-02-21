using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.Basics.Sys.Text
{
    public class DirPathJsonConverter : JsonConverter<DirPath>
    {
        public override DirPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString()?.ToDir() ?? throw new ArgumentException($"Unable to get value from {nameof(reader)}");
        }

        public override void Write(Utf8JsonWriter writer, DirPath value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.RawPath);
        }
    }
}
