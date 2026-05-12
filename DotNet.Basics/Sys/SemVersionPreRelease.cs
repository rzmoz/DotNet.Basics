using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public record SemVersionPreRelease : IComparable<SemVersionPreRelease>, IComparable
    {
        private const string _preReleaseAllowedCharsFormat = @"^[a-zA-Z0-9\.]+$";
        private static readonly Regex _preReleaseAllowedCharsRegex = new(_preReleaseAllowedCharsFormat, RegexOptions.Compiled);

        public SemVersionPreRelease()
            : this([])
        { }

        public SemVersionPreRelease(string? preRelease)
            : this(ParsePreRelease(preRelease))
        { }

        public SemVersionPreRelease(IEnumerable<SemVersionIdentifier> identifiers)
        {
            if (identifiers == null) throw new ArgumentNullException(nameof(identifiers));
            //lowercase identifiers to ignore case and because lower case chars have a higher ascii value than numerics so numerics are always smaller / lower than chars
            Identifiers = identifiers.ToList();

            if (HashBase.Length > 0 && _preReleaseAllowedCharsRegex.IsMatch(HashBase) == false)
                throw new ArgumentOutOfRangeException($"Invalid character(s) found in PreRelease input. ASCII alphanumerics are allowed [a-zA-Z0-9]. Input was: '{HashBase}'");
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public bool Any => Identifiers.Any(i => IsNullOrEmpty(i.ToString()) == false);

        public IReadOnlyList<SemVersionIdentifier> Identifiers { get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public string HashBase => Identifiers
            .Select(i => i.ToString())
            .JoinString(SemVersionLexer.VersionSeparator.ToString())
            .RemovePrefix(SemVersionLexer.VersionSeparator);

        private static IEnumerable<SemVersionIdentifier> ParsePreRelease(string? preRelease)
        {
            return preRelease?.Split(SemVersionLexer.VersionSeparator).Select(i => new SemVersionIdentifier(i)) ?? [];
        }

        private static bool IsNullOrEmpty(string? s) => string.IsNullOrWhiteSpace(s);

        public static bool operator <(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            if (a is null || string.IsNullOrWhiteSpace(a.HashBase))
                return false; // a can only be higher or same as b when a is not set

            if (b is null || string.IsNullOrWhiteSpace(b.HashBase))
                return true; // a is always lower if a is set and b is not

            var sharedLength = Math.Min(a.Identifiers.Count, b.Identifiers.Count);
            for (var i = 0; i < sharedLength; i++)
            {
                var aIdentifier = a.Identifiers[i];
                var bIdentifier = b.Identifiers[i];

                if (aIdentifier == bIdentifier)
                    continue;

                return aIdentifier < bIdentifier;
            }
            return a.Identifiers.Count < b.Identifiers.Count;
        }

        public static bool operator >(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            if (b is null || string.IsNullOrWhiteSpace(b.HashBase))
                return false; // a can only be lower or same as b when b is not set
            if (a is null || string.IsNullOrWhiteSpace(a.HashBase))
                return true; // a is always higher if b is set and a is not

            var sharedLength = Math.Min(a.Identifiers.Count, b.Identifiers.Count);
            for (var i = 0; i < sharedLength; i++)
            {
                var aIdentifier = a.Identifiers[i];
                var bIdentifier = b.Identifiers[i];

                if (aIdentifier == bIdentifier)
                    continue;

                return aIdentifier > bIdentifier;
            }
            return a.Identifiers.Count > b.Identifiers.Count;
        }

        // Custom equality: compare on HashBase (preserves case-insensitive semantics from the original).
        public virtual bool Equals(SemVersionPreRelease? other)
        {
            if (other is null) return false;
            return HashBase.Equals(other.HashBase, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(HashBase);
        }

        public int CompareTo(SemVersionPreRelease? other)
        {
            if (other is null) return 1;
            if (this < other) return -1;
            if (this > other) return 1;
            return 0;
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is SemVersionPreRelease other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(SemVersionPreRelease)}", nameof(obj));
        }

        public override string ToString()
        {
            return HashBase;
        }
    }
}
