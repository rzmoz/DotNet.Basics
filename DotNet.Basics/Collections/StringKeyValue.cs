using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNet.Basics.Collections
{
    [DataContract]
    public sealed class StringKeyValue
    {
        public StringKeyValue() { }
        public StringKeyValue(KeyValuePair<string, string> kvp)
            : this(kvp.Key, kvp.Value)
        {
        }
        public StringKeyValue(string key = null, string value = null)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }

        public static implicit operator StringKeyValue(KeyValuePair<string, string> kvp)
        {
            var kv = new StringKeyValue(kvp.Key, kvp.Value);
            return kv;
        }
        public static implicit operator KeyValuePair<string, string>(StringKeyValue kv)
        {
            return new KeyValuePair<string, string>(kv.Key, kv.Value);
        }

        private bool Equals(StringKeyValue other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {

            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringKeyValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(StringKeyValue left, StringKeyValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringKeyValue left, StringKeyValue right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{{\"{Key}\":\"{Value}\"}}";
        }
    }
}
