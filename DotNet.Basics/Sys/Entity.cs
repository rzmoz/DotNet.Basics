using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public class Entity
    {
        private const string _keyPattern = @"[^a-z0-9\-]";
        private static readonly Regex _keyRegex = new(_keyPattern, RegexOptions.Compiled);
        protected readonly string _displayName;

        public virtual string Key { get; init; } = string.Empty;
        public virtual string DisplayName
        {
            get => _displayName;
            init
            {
                _displayName = value;
                Key = ToKey(value);
            }
        }

        public static implicit operator Entity(string displayName)
        {
            return new Entity
            {
                DisplayName = displayName
            };
        }

        protected virtual string ToKey(string displayName)
        {
            return string.IsNullOrEmpty(displayName) ? string.Empty : _keyRegex.Replace(displayName.ToLowerInvariant().Replace(" ", "-"), string.Empty);
        }

        public int SortOrder { get; set; }

        protected bool Equals(Entity other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{DisplayName} ({Key})";
        }
    }
}
