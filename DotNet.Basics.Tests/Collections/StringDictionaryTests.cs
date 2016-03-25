using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using DotNet.Basics.Collections;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Collections
{
    [TestFixture]
    public class StringDictionaryTests
    {
        const string _myKey = "myKey";
        const string _myValue = "myValue";
        const string _myKeyThatDoesntExist = "SOMEKEYTHATDOESNTEXIST";

        [Test]
        public void Json_Serialization_ProperJson()
        {
            //arrange 
            var kvc = new StringDictionary(new[] { new StringKeyValue(_myKey, _myValue) });

            //serialize
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(StringDictionary));
            ser.WriteObject(stream1, kvc);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            var json = sr.ReadToEnd();

            //assert
            json.Should().Be("[{\"Key\":\"myKey\",\"Value\":\"myValue\"}]");

            //deserialize
            stream1.Position = 0;
            var newKvc = (StringDictionary)ser.ReadObject(stream1);

            //assert
            newKvc.Count.Should().Be(kvc.Count);
            newKvc.Single().Key.Should().Be(kvc.Single().Key);
            newKvc.Single().Value.Should().Be(kvc.Single().Value);
        }

        [Test]
        public void Ctor_WithDictionaryAsArgument_ArgsAreInCollection()
        {
            var args = new Dictionary<string, string> { { _myKey, _myValue } };

            var kvCollection = new StringDictionary(args);

            kvCollection[_myKey].Should().Be(_myValue);
        }

        [Test]
        public void Enumerator_Enumerate_ItemsAreRetrived()
        {
            var args = new Dictionary<string, string> { { _myKey, _myValue } };

            var kvCollection = new StringDictionary(args);

            kvCollection.Single().Value.Should().Be(_myValue);
        }

        [Test]
        public void Ctor_WithEnumerableAsArgument_NullIsReturned()
        {
            var args = new[] { new StringKeyValue(_myKey, _myValue) };

            var kvCollection = new StringDictionary(args);

            kvCollection[_myKeyThatDoesntExist].Should().BeNull();
        }

        [Test]
        public void Ctor_KeyModeNullIfNotFound_NullIsReturned()
        {
            var kvCollection = new StringDictionary(KeyNotFoundMode.ReturnNull) { [_myKey] = _myValue };

            kvCollection[_myKeyThatDoesntExist].Should().BeNull();
        }

        [Test]
        public void Ctor_KeyModeNotFoundExceptionfNotFound_ExceptionIsThrown()
        {
            var kvCollection = new StringDictionary(KeyNotFoundMode.ThroweyNotFoundException) { [_myKey] = _myValue };

            Action action = () => { var @value = kvCollection[_myKeyThatDoesntExist]; };

            action.ShouldThrow<KeyNotFoundException>();
        }


        [Test]
        public void SetIndexer_KeyDoesNotExist_ValueIsAdded()
        {
            var kvCollection = new StringDictionary { [_myKey] = _myValue };

            kvCollection[_myKey].Should().Be(_myValue);
        }
        [Test]
        public void SetIndexer_KeyAlreadyExists_ValueIsUpdated()
        {
            var kvCollection = new StringDictionary { [_myKey] = _myValue };

            var newValue = _myValue + "asd";
            kvCollection[_myKey] = newValue;

            kvCollection[_myKey].Should().Be(newValue);
        }

        [Test]
        public void Add_KeyDoesNotExist_ValueIsAdded()
        {
            var kvCollection = new StringDictionary();

            kvCollection.Add(_myKey, _myValue);

            kvCollection[_myKey].Should().Be(_myValue);
        }

        [Test]
        public void Count_Count_ItemsAreCounted()
        {
            var kvCollection = new StringDictionary();

            const int count = 10;

            for (var i = 0; i < count; i++)
            {
                kvCollection.Add(i.ToString(), _myValue);
            }
            kvCollection.Count.Should().Be(count);
        }

        [Test]
        public void CastOperator_CastToDictionary_Equals()
        {
            var kvc = new StringDictionary { new StringKeyValue(_myKey, _myValue) };
            var dic = new Dictionary<string, string> { { _myKey, _myValue } };

            kvc.Should().BeEquivalentTo((StringDictionary)dic);
        }

        [Test]
        public void ToString_Formatting_StringIsJson()
        {
            var kvc = new StringDictionary { new StringKeyValue(_myKey, _myValue) };
            var json = kvc.ToString();

            json.Should().Be("[{\"Key\":\"myKey\",\"Value\":\"myValue\"}]");
        }
    }
}
