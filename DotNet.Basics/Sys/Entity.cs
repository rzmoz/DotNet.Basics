namespace DotNet.Basics.Sys
{
    public class Entity
    {
        public virtual string Key { get; init; } = string.Empty;
        public virtual string DisplayName { get; init; } = string.Empty;
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
