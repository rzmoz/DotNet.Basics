using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.Basics.IO;

namespace DotNet.Basics.Sys.Text
{
    public class DirPathJsonConverter : JsonConverter<DirPath>
    {
        public override DirPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString().ToDir();
        }

        public override void Write(Utf8JsonWriter writer, DirPath value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.RawPath);
        }
    }
}
