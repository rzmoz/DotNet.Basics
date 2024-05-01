using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public class Dto
    {
        private const string _trimKeyPattern = @"[^ a-zA-Z0-9\.]";
        private static readonly Regex _trimKeyRegex = new(_trimKeyPattern, RegexOptions.Compiled);
        private string _displayName;

        public virtual string Key { get; set; } = string.Empty;

        public virtual string DisplayName
        {
            get => _displayName;
            set
            {
                Key = ToKey(value);
                _displayName = value;
            }
        }

        public static implicit operator Dto(string displayName)
        {
            return new Dto
            {
                DisplayName = displayName
            };
        }

        public int SortOrder { get; set; }

        protected virtual string ToKey(string value)
        {
            return _trimKeyRegex.Replace(value, string.Empty).Trim().ToLowerInvariant().Replace(" ", "-");
        }

        protected bool Equals(Dto other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Dto)obj);
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
