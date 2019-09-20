using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.Basics.Sys.Text
{
    public class SemVersionJsonConverter : JsonConverter<SemVersion>
    {
        public override SemVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return SemVersion.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, SemVersion value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.SemVer20String);
        }
    }
}
