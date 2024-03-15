using System;
using System.Collections.Generic;

namespace DotNet.Basics.Collections
{
    public class StringDictionaryOptions<T>
    {
        public KeyNotFound KeyNotFound { get; set; } = KeyNotFound.ThrowException;
        public KeyLookup KeyLookup { get; set; } = KeyLookup.CaseSensitive;

        public virtual string GetKey(string key)
        {
            return GetKeyFunc(KeyLookup)(key);
        }
        public virtual T GetValue(string key, IDictionary<string, T> dic)
        {
            return GetValueFunc(KeyNotFound, dic)(key);
        }
        public virtual void SetValue(string key, T item, IDictionary<string, T> dic)
        {
            dic[GetKey(key)] = item;
        }

        protected virtual Func<string, string> GetKeyFunc(KeyLookup keyLookup)
        {
            return keyLookup switch
            {
                KeyLookup.IgnoreCase => key => key.ToLowerInvariant(),
                KeyLookup.CaseSensitive => key => key,
                _ => throw new ArgumentOutOfRangeException(nameof(keyLookup), keyLookup, null)
            };
        }
        protected virtual Func<string, T> GetValueFunc(KeyNotFound keyNotFound, IDictionary<string, T> dic)
        {
            return keyNotFound switch
            {
                KeyNotFound.ThrowException => key => dic[GetKey(key)],
                KeyNotFound.ReturnDefault => key => dic.TryGetValue(GetKey(key), out var value) ? value : default,
                _ => throw new ArgumentException($"{nameof(KeyNotFound)} Not supported: {keyNotFound}")
            };
        }

    }
}
