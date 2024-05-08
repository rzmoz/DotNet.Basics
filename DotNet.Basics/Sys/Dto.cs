namespace DotNet.Basics.Sys
{
    public class Dto
    {
        private string _displayName;
        private string _key;

        private delegate void StringPropertyUpdated(string value);

        private event StringPropertyUpdated _displayNameUpdated;
        private event StringPropertyUpdated _keyUpdated;

        public Dto()
        {
            _displayNameUpdated += DisplayNameUpdated;
            _keyUpdated += KeyUpdated;
        }

        public virtual string Key
        {
            get => _key;
            set
            {
                _key = value;
                _keyUpdated?.Invoke(value);
            }
        }

        public virtual string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                _displayNameUpdated?.Invoke(value);
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

        protected virtual void KeyUpdated(string key)
        {
        }

        protected virtual void DisplayNameUpdated(string displayName)
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
