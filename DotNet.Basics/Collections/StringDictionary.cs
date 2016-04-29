using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class StringDictionary : StringKeyDictionary<string>
    {
        public StringDictionary()
        {
        }

        public StringDictionary(DictionaryKeyMode dictionaryKeyMode = DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException) : base(dictionaryKeyMode, keyNotFoundMode)
        {
        }

        public StringDictionary(IEnumerable<KeyValuePair<string, string>> keyValues, DictionaryKeyMode dictionaryKeyMode = DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException) : base(keyValues, dictionaryKeyMode, keyNotFoundMode)
        {
        }
        
        public StringDictionary(IEnumerable<StringPair> keyValues, DictionaryKeyMode dictionaryKeyMode = DictionaryKeyMode.KeyCaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : this(keyValues.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value)), dictionaryKeyMode, keyNotFoundMode)
        {
        }
    }
}
