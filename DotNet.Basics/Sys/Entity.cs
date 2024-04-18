using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public class Entity
    {
        private const string _trimKeyPattern = @"[^ a-zA-Z0-0\.]";
        private static readonly Regex _trimKeyRegex = new(_trimKeyPattern, RegexOptions.Compiled);
        private readonly string _displayName;

        public virtual string Key { get; init; } = string.Empty;

        public virtual string DisplayName
        {
            get => _displayName;
            init
            {
                Key = ToKey(value);
                _displayName = value;
            }
        }

        public static implicit operator Entity(string displayName)
        {
            return new Entity
            {
                DisplayName = displayName
            };
        }

        public int SortOrder { get; set; }

        protected virtual string ToKey(string value)
        {
            return _trimKeyRegex.Replace(value, string.Empty).Trim().ToLowerInvariant().Replace(" ", "-");
        }

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
