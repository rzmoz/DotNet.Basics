using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.Basics.IO;

namespace DotNet.Basics.Sys.Text
{
    public class PathInfoJsonConverter : JsonConverter<PathInfo>
    {
        public override PathInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString().ToPath();
        }

        public override void Write(Utf8JsonWriter writer, PathInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.RawPath);
        }
    }
}
