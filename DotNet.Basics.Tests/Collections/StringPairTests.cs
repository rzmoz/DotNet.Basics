using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class StringKeyValueTests
    {
        const string _myKey = "myKey";
        const string _myValue = "myValue";

        [Fact]
        public void Json_Serialization_ProperJson()
        {
            //arrange 
            var kv = new StringPair(_myKey, _myValue);

            //serialize
            var stream1 = new System.IO.MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(StringPair));
            ser.WriteObject(stream1, kv);
            stream1.Position = 0;
            var sr = new System.IO.StreamReader(stream1);
            var json = sr.ReadToEnd();

            //assert
            json.Should().Be("{\"Key\":\"myKey\",\"Value\":\"myValue\"}");

            //deserialize
            stream1.Position = 0;
            var newKv = (StringPair)ser.ReadObject(stream1);

            //assert
            newKv.Key.Should().Be(kv.Key);
            newKv.Value.Should().Be(kv.Value);
        }

        [Fact]
        public void Ctor_Default_AllIsNull()
        {
            var kv = new StringPair();
            kv.Key.Should().BeNull();
            kv.Value.Should().BeNull();
        }

        [Fact]
        public void Ctor_Values_ValuesAreSet()
        {
            var kv = new StringPair(_myKey, _myValue);
            kv.Key.Should().Be(_myKey);
            kv.Value.Should().Be(_myValue);
        }
        [Fact]
        public void Equality_RefAndEquals_AreEquals()
        {
            var kv1 = new StringPair(_myKey, _myValue);
            var kv2 = new StringPair(_myKey, _myValue);
            (kv1 == kv2).Should().BeTrue();
            (kv2 == kv1).Should().BeTrue();
            kv1.Equals(kv2).Should().BeTrue();
            kv1.Equals(kv2).Should().BeTrue();
        }

        [Fact]
        public void ToString_DebugInfo_KeyAndValueIsOutput()
        {
            var kv1 = new StringPair(_myKey, _myValue);

            kv1.ToString().Should().Be($"{{\"{_myKey}\":\"{_myValue}\"}}");
        }

        [Fact]
        public void CastOperator_CastToKeyValuePair_Equals()
        {
            var kv = new StringPair(_myKey, _myValue);
            var kvp = new KeyValuePair<string, string>(_myKey, _myValue);

            kv.Should().Be((StringPair)kvp);
        }
    }
}
