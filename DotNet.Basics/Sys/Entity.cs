using System;
using System.Collections.Generic;

namespace DotNet.Basics.Sys
{
    public class Entity : IComparable<Entity>, IComparer<Entity>
    {
        public string Key { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public int SortOrder { get; set; }

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
