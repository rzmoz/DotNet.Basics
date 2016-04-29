using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNet.Basics.Collections
{
    [DataContract]
    public sealed class StringPair
    {
        public StringPair() { }
        public StringPair(KeyValuePair<string, string> kvp)
            : this(kvp.Key, kvp.Value)
        {
        }
        public StringPair(string key = null, string value = null)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }

        public static explicit operator StringPair(KeyValuePair<string, string> kvp)
        {
            var kv = new StringPair(kvp.Key, kvp.Value);
            return kv;
        }
        public static explicit operator KeyValuePair<string, string>(StringPair kv)
        {
            return new KeyValuePair<string, string>(kv.Key, kv.Value);
        }

        private bool Equals(StringPair other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {

            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringPair)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(StringPair left, StringPair right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringPair left, StringPair right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{{\"{Key}\":\"{Value}\"}}";
        }
    }
}
