using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class KeyValueTests
    {
        const string _myKey = "myKey";
        const string _myValue = "myValue";

        [Test]
        public void Json_Serialization_ProperJson()
        {
            //arrange 
            var kv = new KeyValue(_myKey, _myValue);

            //serialize
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(KeyValue));
            ser.WriteObject(stream1, kv);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            var json = sr.ReadToEnd();

            //assert
            json.Should().Be("{\"Key\":\"myKey\",\"Value\":\"myValue\"}");

            //deserialize
            stream1.Position = 0;
            var newKv = (KeyValue)ser.ReadObject(stream1);

            //assert
            newKv.Key.Should().Be(kv.Key);
            newKv.Value.Should().Be(kv.Value);
        }

        [Test]
        public void Ctor_Default_AllIsNull()
        {
            var kv = new KeyValue();
            kv.Key.Should().BeNull();
            kv.Value.Should().BeNull();
        }

        [Test]
        public void Ctor_Values_ValuesAreSet()
        {
            var kv = new KeyValue(_myKey, _myValue);
            kv.Key.Should().Be(_myKey);
            kv.Value.Should().Be(_myValue);
        }
        [Test]
        public void Equality_RefAndEquals_AreEquals()
        {
            var kv1 = new KeyValue(_myKey, _myValue);
            var kv2 = new KeyValue(_myKey, _myValue);
            (kv1 == kv2).Should().BeTrue();
            (kv2 == kv1).Should().BeTrue();
            kv1.Equals(kv2).Should().BeTrue();
            kv1.Equals(kv2).Should().BeTrue();
        }

        [Test]
        public void ToString_DebugInfo_KeyAndValueIsOutput()
        {
            var kv1 = new KeyValue(_myKey, _myValue);

            kv1.ToString().Should().Be($"{{\"{_myKey}\":\"{_myValue}\"}}");
        }

        [Test]
        public void CastOperator_CastToKeyValuePair_Equals()
        {
            var kv = new KeyValue(_myKey, _myValue);
            var kvp = new KeyValuePair<string, string>(_myKey, _myValue);

            kv.Should().Be((KeyValue)kvp);
        }
    }
}
