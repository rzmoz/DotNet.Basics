using System;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public class SemVersionIdentifier
    {
        private readonly string _hashBase;
        private const string _allNumbersFormat = @"^[0-9]+$";
        private static readonly Regex _allNumbersFormatRegex = new Regex(_allNumbersFormat, RegexOptions.Compiled);

        public SemVersionIdentifier(string identifier)
        {
            Identifier = identifier ?? string.Empty;
            _hashBase = Identifier.ToLowerInvariant();
            IsNumeric = _allNumbersFormatRegex.IsMatch(Identifier);
        }

        public string Identifier { get; }
        public bool IsNumeric { get; }

        public static bool operator ==(SemVersionIdentifier a, SemVersionIdentifier b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(SemVersionIdentifier a, SemVersionIdentifier b)
        {
            return !(a == b);
        }
        public static bool operator <(SemVersionIdentifier a, SemVersionIdentifier b)
        {
            if (ReferenceEquals(null, b))
                return false;//a is only higher or same when b is null, so never lower

            if (ReferenceEquals(null, a) || string.IsNullOrWhiteSpace(a.Identifier))
                return false;//a is never lower when b is set b, so never lower

            if (a.IsNumeric && b.IsNumeric)
                return int.Parse(a._hashBase) < int.Parse(b._hashBase);

            return string.Compare(a._hashBase, b._hashBase, StringComparison.Ordinal) < 0;
        }
        public static bool operator >(SemVersionIdentifier a, SemVersionIdentifier b)
        {
            if (ReferenceEquals(null, a))
                return false;//a is only lower or same when b is null, so never higher

            if (ReferenceEquals(null, b) || string.IsNullOrWhiteSpace(b.Identifier))
                return true;//a is always higher when b is not present

            if (a.IsNumeric && b.IsNumeric)
                return int.Parse(a._hashBase) > int.Parse(b._hashBase);

            return string.Compare(a._hashBase, b._hashBase, StringComparison.Ordinal) > 0;
        }
        protected bool Equals(SemVersionIdentifier other)
        {
            return other._hashBase.Equals(_hashBase, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SemVersionIdentifier)obj);
        }

        public override int GetHashCode()
        {
            return _hashBase.GetHashCode();
        }

        public int CompareTo(SemVersionIdentifier other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }

        public override string ToString()
        {
            return _hashBase;
        }
    }
}
