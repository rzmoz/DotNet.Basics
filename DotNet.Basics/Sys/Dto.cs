using System;

namespace DotNet.Basics.Sys
{
    public class Dto
    {
        private readonly EventHandler<string> _displayNameUpdated;

        public Dto()
        {
            _displayNameUpdated += DisplayNameUpdated;
        }

        private string _displayName;

        public virtual string Key { get; set; } = string.Empty;
        public virtual string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                _displayNameUpdated?.Invoke(this, _displayName);
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

        protected void DisplayNameUpdated(object sender, string displayName)
        {
            if (!string.IsNullOrWhiteSpace(Key))
                return;
            Key = displayName.Trim().ToLowerInvariant().Replace(" ", "-");
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
