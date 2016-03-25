using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNet.Basics.Sys
{
    public class StringKeyDictionary<TValue> : Dictionary<string, TValue>
    {
        public StringKeyDictionary()
            : base()
        { }

        public StringKeyDictionary(int capacity)
            : base(capacity)
        { }
        public StringKeyDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        { }
        public StringKeyDictionary(IDictionary<string, TValue> dictionary)
            : base(dictionary)
        { }
        public StringKeyDictionary(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer)
        { }
        public StringKeyDictionary(IDictionary<string, TValue> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        { }
        protected StringKeyDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
