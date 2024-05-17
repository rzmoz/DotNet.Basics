namespace DotNet.Basics.Sys
{
    public class Dto
    {
        private string _key;
        private string _displayName;

        public virtual string Key
        {
            get => _key;
            set
            {
                _key = PreKeySet(value);
                PostKeySet(_key);
            }
        }

        public virtual string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = PreDisplayNameSet(value);
                PostDisplayNameSet(_displayName);
            }
        }

        public static implicit operator Dto(string displayName)
        {
            return new Dto
            {
                DisplayName = displayName
            };
        }

        public int SortOrder { get; set; } = 0;

        protected virtual string PreKeySet(string key)
        {
            return key;
        }
        protected virtual void PostKeySet(string key)
        { }

        protected virtual string PreDisplayNameSet(string displayName)
        {
            if (string.IsNullOrWhiteSpace(Key))
                Key = displayName.Trim().ToLowerInvariant().Replace(" ", "-");
            return displayName;
        }
        protected virtual void PostDisplayNameSet(string displayName)
        { }

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
