using System;
using System.Collections.Generic;
using DotNet.Basics.Rest;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonSerializer = DotNet.Basics.Rest.JsonSerializer;

namespace DotNet.Basics.Tests.Rest
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private IJsonSerializer _serialiser;

        [SetUp]
        public void SetUp()
        {
            _serialiser = new JsonSerializer();
        }

        [Test]
        public void FromJson_SimpleType_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbc"",""master"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbm"",""web"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbk""}}";

            var dto = _serialiser.Deserialize<ConnectionStringsDto>(connectionStringsJson);

            dto.ConnectionStrings.Count.Should().Be(3);
        }

        [Test]
        public void ConvertTo_ConvertToString_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{'ConnectionStrings':{'core':'user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbc','master':'user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbm','web':'user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbk'}}";

            var rawstring = _serialiser.Serialize(connectionStringsJson);

            rawstring.Should().Be(connectionStringsJson);
        }

        [Test]
        public void Serialize_Dictionary_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbc"",""master"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbm"",""web"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbk""}}";
            var dto = new ConnectionStringsDto
            {
                ConnectionStrings = new Dictionary<string, string> { { "core", "user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbc" }, { "master", "user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbm" }, { "web", "user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbk" } }
            };

            var serialized = _serialiser.Serialize(dto);

            serialized.Should().Be(connectionStringsJson);
        }

        [Test]
        public void Serialize_Simple_TypeIsDeSerialized()
        {
            var name = "myName";
            var timestamp = new DateTimeOffset(2001, 01, 01, 01, 01, 01, 0.Seconds()).DateTime;

            const string json = @"{""Name"":""myName"",""TimeStamp"":""2001-01-01T01:01:01""}";
            var dto = new SimpleDto()
            {
                Name = name,
                TimeStamp = timestamp
            };

            var serialized = _serialiser.Serialize(dto);

            serialized.Should().Be(json);
        }

        [Test]
        public void Serialize_Null_TypeIsDeSerialized()
        {
            const string json = @"{}";

            var serialized = _serialiser.Serialize(null);

            serialized.Should().Be(json);
        }

        [Test]
        [TestCase(null)] //null
        [TestCase("")]  //empty
        [TestCase("    ")] //whitespace
        public void Serialize_NullOrWhiteSpaceString_TypeIsDeSerialized(string content)
        {
            const string json = @"{}";

            var serialized = _serialiser.Serialize(content);

            serialized.Should().Be(json);
        }
    }

    public class ConnectionStringsDto
    {
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }

    public class SimpleDto
    {
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
