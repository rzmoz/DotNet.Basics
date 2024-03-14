using System;
using System.Collections.Generic;

namespace DotNet.Basics.Sys
{
    public class Entity : IComparable<Entity>, IComparer<Entity>
    {
        private readonly string _innerKey = string.Empty;
        private readonly string _innerDisplayName = string.Empty;

        public string Key
        {
            get => GetKeyFunc(_innerKey);
            init => _innerKey = SetKeyFunc(value);
        }

        public string DisplayName
        {
            get => GetDisplayNameFunc(_innerDisplayName);
            init => _innerDisplayName = SetDisplayNameFunc(value);
        }

        public int SortOrder { get; set; }

        protected virtual string GetKeyFunc(string value)
        {
            return value;
        }
        protected virtual string SetKeyFunc(string value)
        {
            return value;
        }
        protected virtual string SetDisplayNameFunc(string value)
        {
            return value;
        }
        protected virtual string GetDisplayNameFunc(string value)
        {
            return value;
        }

        protected bool Equals(Entity other)
        {
            return Key.Equals(other.Key);
        }

        public int Compare(Entity x, Entity y)
        {
            return x.CompareTo(y);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public int CompareTo(Entity other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var sortOrderComparison = SortOrder.CompareTo(other.SortOrder);
            if (sortOrderComparison != 0)
                return sortOrderComparison;
            var displayNameComparison = string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
            if (displayNameComparison != 0)
                return displayNameComparison;
            return string.Compare(Key, other.Key, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"{DisplayName} ({Key})";
        }
    }
}
