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
        private readonly IDictionary<string, string> _dic;

        private readonly Func<string, string> _getvalueMethod;

        public StringDictionary()
            : this(KeyNotFoundMode.ReturnNull, KeyMode.CaseInsensitive)
        {
        }

        public StringDictionary(KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ReturnNull, KeyMode keyMode = KeyMode.CaseInsensitive)
        {
            KeyNotFoundMode = keyNotFoundMode;
            KeyMode = keyMode;
            if (keyNotFoundMode == KeyNotFoundMode.ReturnNull)
                _getvalueMethod = GetNullIfNotFound;
            else
                _getvalueMethod = GetKeyNotFoundExceptionIfNotFound;

            if (keyMode == KeyMode.CaseInsensitive)
                _dic = new StringKeyCaseInsensitiveDictionary<string>();
            else
                _dic = new StringKeyDictionary<string>();
        }

        public StringDictionary(IEnumerable<StringKeyValue> keyValues, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ReturnNull, KeyMode keyMode = KeyMode.CaseInsensitive)
            : this(keyNotFoundMode, keyMode)
        {
            if (keyValues == null)
                return;
            foreach (var keyValue in keyValues)
                _dic.Add(keyValue);
        }
        public StringDictionary(IEnumerable<KeyValuePair<string, string>> keyValues, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ReturnNull, KeyMode keyMode = KeyMode.CaseInsensitive)
            : this(keyValues.Select(kv => new StringKeyValue(kv.Key, kv.Value)), keyNotFoundMode, keyMode)
        {
        }

        public string this[string key]
        {
            get { return _getvalueMethod(key); }
            set { _dic[key] = value; }
        }

        public KeyNotFoundMode KeyNotFoundMode { get; }
        public KeyMode KeyMode { get; }

        public void Add(string key, string value)
        {
            _dic.Add(key, value);
        }
        public void Add(StringKeyValue item)
        {
            _dic.Add(item.Key, item.Value);
        }
        public bool Remove(StringKeyValue item)
        {
            return _dic.Remove(item.Key);
        }

        public IEnumerator<StringKeyValue> GetEnumerator()
        {
            return _dic.Select(kv => new StringKeyValue(kv.Key, kv.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dic.Count;

        public override string ToString()
        {
            using (MemoryStream stream1 = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(IDictionary<string, string>));
                ser.WriteObject(stream1, _dic);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

        private string GetNullIfNotFound(string key)
        {
            string @value;
            if (_dic.TryGetValue(key, out @value))
                return @value;
            return null;
        }

        private string GetKeyNotFoundExceptionIfNotFound(string key)
        {
            return _dic[key];
        }
    }
}
