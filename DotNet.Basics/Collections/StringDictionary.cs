using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public class StringDictionary : StringKeyDictionary<string>
    {
        public StringDictionary()
        {
        }

        public StringDictionary(KeyMode keyMode = KeyMode.CaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : base(keyMode, keyNotFoundMode)
        {
        }

        public StringDictionary(IEnumerable<StringPair> keyValues, KeyMode keyMode = KeyMode.CaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : base(keyValues.Select(sp => new KeyValuePair<string, string>(sp.Key, sp.Value)), keyMode, keyNotFoundMode)
        {
        }

        public StringDictionary(IEnumerable<KeyValuePair<string, string>> keyValues, KeyMode keyMode = KeyMode.CaseSensitive, KeyNotFoundMode keyNotFoundMode = KeyNotFoundMode.ThrowKeyNotFoundException)
            : base(keyValues, keyMode, keyNotFoundMode)
        {
        }
    }
}
