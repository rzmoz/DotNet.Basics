using System;
using System.Collections.Generic;
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
            var kvc = new StringDictionary(new StringPair(_myKey, _myValue).ToEnumerable());

            //serialize
            var stream1 = new System.IO.MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(StringDictionary));
            ser.WriteObject(stream1, kvc);
            stream1.Position = 0;
            var  sr = new System.IO.StreamReader(stream1);
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
            IEnumerable<StringPair> args = new[] { new StringPair(_myKey, _myValue) };

            var kvCollection = new StringDictionary(args, DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode.ThrowKeyNotFoundException);

            kvCollection.Single().Value.Should().Be(_myValue);
        }

        [Test]
        public void Ctor_WithEnumerableAsArgument_ThrowsException()
        {
            var args = new[] { new StringPair(_myKey, _myValue) };

            var kvCollection = new StringDictionary(args);

            Action action = () => { var @value = kvCollection[_myKeyThatDoesntExist]; };

            action.ShouldThrow<KeyNotFoundException>();
        }

        [Test]
        public void Ctor_KeyModeNullIfNotFound_NullIsReturned()
        {
            var kvCollection = new StringDictionary(DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode.ReturnDefault) { [_myKey] = _myValue };

            kvCollection[_myKeyThatDoesntExist].Should().BeNull();
        }

        [Test]
        public void Ctor_KeyModeNotFoundExceptionfNotFound_ExceptionIsThrown()
        {
            var kvCollection = new StringDictionary(DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode.ThrowKeyNotFoundException) { [_myKey] = _myValue };

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
        public void ToString_Formatting_StringIsJson()
        {
            var kvc = new StringDictionary(new StringPair(_myKey, _myValue).ToEnumerable());
            var json = kvc.ToString();

            json.Should().Be("[{\"Key\":\"myKey\",\"Value\":\"myValue\"}]");
        }
    }
}
