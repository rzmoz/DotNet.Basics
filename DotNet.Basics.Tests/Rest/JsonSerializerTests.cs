using System;
using System.Collections.Generic;
using DotNet.Basics.Rest;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Rest
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private JsonRestSerializer _serializer;

        [SetUp]
        public void SetUpFixture()
        {
            _serializer = new JsonRestSerializer();
        }

        [Test]
        public void FromJson_SimpleType_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbc"",""master"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbm"",""web"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbk""}}";

            var dto = _serializer.FromJson<ConnectionStringsDto>(connectionStringsJson);

            dto.ConnectionStrings.Count.Should().Be(3);
        }

        [Test]
        public void ConvertTo_ConvertToString_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbc"",""master"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbm"",""web"":""user id=sc;password=pw123;Data Source=t0hmavsbyz.database.windows.net;Database=dbk""}}";

            var rawstring = _serializer.ConvertTo<string>(connectionStringsJson);

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

            var serialized = _serializer.Serialize(dto);

            serialized.Should().Be(connectionStringsJson);
        }

        [Test]
        public void Serialize_Simple_TypeIsDeSerialized()
        {
            var name = "myName";
            var timestamp = new DateTimeOffset(2001, 01, 01, 01, 01, 01, 0.Seconds()).DateTime;
            
            const string json = @"{""Name"":""myName"",""TimeStamp"":""\/Date(978282061000)\/""}";
            var dto = new SimpleDto()
            {
                Name = name,
                TimeStamp = timestamp
            };

            var serialized = _serializer.Serialize(dto);

            serialized.Should().Be(json);
        }

        [Test]
        public void Serialize_Null_TypeIsDeSerialized()
        {
            const string json = @"{}";
            
            var serialized = _serializer.Serialize(null);

            serialized.Should().Be(json);
        }

        [Test]
        [TestCase(null)] //null
        [TestCase("")]  //empty
        [TestCase("    ")] //whitespace
        public void Serialize_NullOrWhiteSpaceString_TypeIsDeSerialized(string content)
        {
            const string json = @"{}";

            var serialized = _serializer.Serialize(content);

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
