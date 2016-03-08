using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace DotNet.Basics.Sys
{
    public class KeyValueCollection : IReadOnlyCollection<KeyValue>
    {
        private readonly IDictionary<string, string> _keyValues;
        private readonly Func<string, string> _getvalueMethod;

        public KeyValueCollection()
            : this(KeyMode.NullIfNotFound)
        {
        }

        public KeyValueCollection(KeyMode keyMode)
        {
            KeyMode = keyMode;
            if (keyMode == KeyMode.NullIfNotFound)
                _getvalueMethod = GetNullIfNotFound;
            else
                _getvalueMethod = GetNotFoundExceptionIfNotFound;

            _keyValues = new Dictionary<string, string>();
        }

        public KeyValueCollection(IEnumerable<KeyValue> keyValues, KeyMode keyMode = KeyMode.NullIfNotFound)
            : this(keyMode)
        {
            if (keyValues == null)
                return;
            foreach (var keyValue in keyValues)
            {
                _keyValues.Add(keyValue);
            }
        }

        public KeyValueCollection(IDictionary<string, string> keyValues, KeyMode keyMode = KeyMode.NullIfNotFound)
            : this(keyMode)
        {
            if (keyValues == null)
                return;
            _keyValues = keyValues;
        }

        public KeyValueCollection(params KeyValue[] keyValues)
            : this(keyValues, KeyMode.NullIfNotFound)
        {
        }

        public static implicit operator KeyValueCollection(Dictionary<string, string> kvd)
        {
            return new KeyValueCollection(kvd);
        }
        public static implicit operator Dictionary<string, string>(KeyValueCollection kvc)
        {
            return new Dictionary<string, string>(kvc._keyValues);
        }

        public string this[string key]
        {
            get { return _getvalueMethod(key); }
            set { _keyValues[key] = value; }
        }

        public KeyMode KeyMode { get; }

        public void Add(string key, string value)
        {
            _keyValues.Add(key, value);
        }
        public void Add(KeyValue item)
        {
            _keyValues.Add(item.Key, item.Value);
        }
        public bool Remove(KeyValue item)
        {
            return _keyValues.Remove(item.Key);
        }

        public IEnumerator<KeyValue> GetEnumerator()
        {
            return _keyValues.Select(kv => new KeyValue(kv.Key, kv.Value)).GetEnumerator();
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

        private string GetNullIfNotFound(string key)
        {
            string @value;
            if (_keyValues.TryGetValue(key, out @value))
                return @value;
            return null;
        }

        private string GetNotFoundExceptionIfNotFound(string key)
        {
            return _keyValues[key];
        }
    }
}
