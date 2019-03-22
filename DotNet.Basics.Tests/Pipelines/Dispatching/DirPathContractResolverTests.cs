using DotNet.Basics.IO;
using DotNet.Basics.Pipelines.Dispatching;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines.Dispatching
{
    public class DirPathContractResolverTests
    {
        [Fact]
        public void Serialize_IgnoreParentProperty_ParentIsIgnored()
        {
            var dir = "c:\\a\\b\\c\\d".ToDir();

            var jsonResolver = new DirPathContractResolver();
            var jsonSettings = new JsonSerializerSettings { ContractResolver = jsonResolver };

            //act
            var json = JsonConvert.SerializeObject(dir, Formatting.Indented, jsonSettings);

            //assert
            json.Should().NotContain("\"Parent\"");
        }
    }
}
