using System;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public record SemVersionIdentifier : IComparable<SemVersionIdentifier>, IComparable
    {
        private const string _allNumbersFormat = @"^[0-9]+$";
        private static readonly Regex _allNumbersFormatRegex = new(_allNumbersFormat, RegexOptions.Compiled);

        private readonly string _hashBase;

        public SemVersionIdentifier(string identifier)
        {
            Identifier = identifier ?? string.Empty;
            _hashBase = Identifier.ToLowerInvariant();
            IsNumeric = _allNumbersFormatRegex.IsMatch(Identifier);
        }

        public string Identifier { get; }
        public bool IsNumeric { get; }

        public static bool operator <(SemVersionIdentifier? a, SemVersionIdentifier? b)
        {
            if (b is null)
                return false; // a is only higher or same when b is null, so never lower

            if (a is null || string.IsNullOrWhiteSpace(a.Identifier))
                return false; // a is never lower when b is set, so never lower

            if (a.IsNumeric && b.IsNumeric)
                return CompareNumericIdentifiers(a._hashBase, b._hashBase) < 0;

            return string.Compare(a._hashBase, b._hashBase, StringComparison.Ordinal) < 0;
        }

        public static bool operator >(SemVersionIdentifier? a, SemVersionIdentifier? b)
        {
            if (a is null)
                return false; // a is only lower or same when a is null, so never higher

            if (b is null || string.IsNullOrWhiteSpace(b.Identifier))
                return true; // a is always higher when b is not present

            if (a.IsNumeric && b.IsNumeric)
                return CompareNumericIdentifiers(a._hashBase, b._hashBase) > 0;

            return string.Compare(a._hashBase, b._hashBase, StringComparison.Ordinal) > 0;
        }

        // Spec-correct numeric identifier comparison (SemVer 2.0 §11):
        // Numeric identifiers compare numerically. Length-first then lexicographic on the
        // raw digits is equivalent to numeric comparison without overflow risk.
        // Both inputs are already known to be non-empty all-digit strings via IsNumeric.
        private static int CompareNumericIdentifiers(string a, string b)
        {
            // Strip leading zeros for length-first comparison. SemVer 2.0 disallows
            // leading zeros in numeric identifiers, but tolerate defensively.
            var aTrimmed = a.TrimStart('0');
            var bTrimmed = b.TrimStart('0');

            if (aTrimmed.Length == 0) aTrimmed = "0";
            if (bTrimmed.Length == 0) bTrimmed = "0";

            if (aTrimmed.Length != bTrimmed.Length)
                return aTrimmed.Length.CompareTo(bTrimmed.Length);

            return string.CompareOrdinal(aTrimmed, bTrimmed);
        }

        // Equality is case-insensitive on the identifier; preserve original casing for display.
        public virtual bool Equals(SemVersionIdentifier? other)
        {
            if (other is null) return false;
            return _hashBase.Equals(other._hashBase, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(_hashBase);
        }

        public int CompareTo(SemVersionIdentifier? other)
        {
            if (other is null) return 1;
            if (this < other) return -1;
            if (this > other) return 1;
            return 0;
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is SemVersionIdentifier other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(SemVersionIdentifier)}", nameof(obj));
        }

        public override string ToString()
        {
            return _hashBase;
        }
    }
}
