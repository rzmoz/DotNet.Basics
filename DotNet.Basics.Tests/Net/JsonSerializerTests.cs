using System.Collections.Generic;
using DotNet.Basics.Net;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Net
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private JsonSerializer _serializer;

        [SetUp]
        public void SetUpFixture()
        {
            _serializer = new JsonSerializer();
        }

        [Test]
        public void FromJson_SimpleType_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Core"",""master"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Master"",""web"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Web""}}";

            var dto = _serializer.FromJson<ConnectionStringsDto>(connectionStringsJson);

            dto.ConnectionStrings.Count.Should().Be(3);
        }

        [Test]
        public void ConvertTo_ConvertToString_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Core"",""master"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Master"",""web"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Web""}}";

            var rawstring = _serializer.ConvertTo<string>(connectionStringsJson);

            rawstring.Should().Be(connectionStringsJson);
        }

        [Test]
        public void Serialize_ConvertToString_TypeIsDeSerialized()
        {
            const string connectionStringsJson = @"{""ConnectionStrings"":{""core"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Core"",""master"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Master"",""web"":""user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Web""}}";
            var dto = new ConnectionStringsDto
            {
                ConnectionStrings = new Dictionary<string, string> { { "core", "user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Core" }, { "master", "user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Master" }, { "web", "user id=sc;password=Sitecore123;Data Source=t0hmavsbyz.database.windows.net;Database=Sc72_Web" } }
            };

            var serialized = _serializer.Serialize(dto);
            
            serialized.Should().Be(connectionStringsJson);
        }
    }

    public class ConnectionStringsDto
    {
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}
