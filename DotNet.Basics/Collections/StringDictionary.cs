using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace DotNet.Basics.Collections
{
    public class StringDictionary : IReadOnlyCollection<StringKeyValue>
    {
        private readonly IDictionary<string, string> _keyValues;
        private readonly Func<string, string> _getvalueMethod;

        public StringDictionary()
            : this(KeyNotFoundMode.ReturnNull, KeyMode.CaseInsensitive)
        {
        }

        public StringDictionary(KeyNotFoundMode keyNotFoundMode=KeyNotFoundMode.ReturnNull, KeyMode keyMode=KeyMode.CaseInsensitive)
        {
            KeyNotFoundMode = keyNotFoundMode;
            KeyMode = keyMode;
            if (keyNotFoundMode == KeyNotFoundMode.ReturnNull)
                _getvalueMethod = GetNullIfNotFoundCaseSensitive;
            else
                _getvalueMethod = GetKeyNotFoundExceptionIfNotFoundCaseSensistive;

            _keyValues = new Dictionary<string, string>();
        }

        public StringDictionary(IEnumerable<StringKeyValue> keyValues, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ReturnNull, KeyMode keyMode = KeyMode.CaseInsensitive)
            : this(keyNotFoundMode, keyMode)
        {
            if (keyValues == null)
                return;
            foreach (var keyValue in keyValues)
                _keyValues.Add(keyValue);
        }

        public StringDictionary(IDictionary<string, string> keyValues, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ReturnNull, KeyMode keyMode = KeyMode.CaseInsensitive)
            : this(keyNotFoundMode, keyMode)
        {
            if (keyValues == null)
                return;
            _keyValues = keyValues;
        }

        public static implicit operator StringDictionary(Dictionary<string, string> kvd)
        {
            return new StringDictionary(kvd);
        }
        public static implicit operator Dictionary<string, string>(StringDictionary kvc)
        {
            return new Dictionary<string, string>(kvc._keyValues);
        }

        public string this[string key]
        {
            get { return _getvalueMethod(key); }
            set { _keyValues[key] = value; }
        }

        public KeyNotFoundMode KeyNotFoundMode { get; }
        public KeyMode KeyMode { get; }

        public void Add(string key, string value)
        {
            _keyValues.Add(key, value);
        }
        public void Add(StringKeyValue item)
        {
            _keyValues.Add(item.Key, item.Value);
        }
        public bool Remove(StringKeyValue item)
        {
            return _keyValues.Remove(item.Key);
        }

        public IEnumerator<StringKeyValue> GetEnumerator()
        {
            return _keyValues.Select(kv => new StringKeyValue(kv.Key, kv.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _keyValues.Count;

        public override string ToString()
        {
            using (MemoryStream stream1 = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(IDictionary<string, string>));
                ser.WriteObject(stream1, _keyValues);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

        private string GetNullIfNotFoundCaseSensitive(string key)
        {
            string @value;
            if (_keyValues.TryGetValue(key, out @value))
                return @value;
            return null;
        }
        private string GetNullIfNotFoundCaseInsensitive(string key)
        {
            string @value;
            if (_keyValues.TryGetValue(key, out @value))
                return @value;
            return null;
        }


        private string GetKeyNotFoundExceptionIfNotFoundCaseSensistive(string key)
        {
            return _keyValues[key];
        }
        private string GetKeyNotFoundExceptionIfNotFoundCaseInsensistive(string key)
        {
            return _keyValues[key];
        }
    }
}
