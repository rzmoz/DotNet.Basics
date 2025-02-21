using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public class SemVersionPreRelease
    {
        private const string _preReleaseAllowedCharsFormat = @"^[a-zA-Z0-9\.]+$";
        private static readonly Regex _preReleaseAllowedCharsRegex = new (_preReleaseAllowedCharsFormat, RegexOptions.Compiled);

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
        public bool Any => Identifiers.Any(i => string.IsNullOrWhiteSpace(i.ToString()) == false);

        public IReadOnlyList<SemVersionIdentifier> Identifiers { get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public string HashBase => Identifiers.Select(i => i.ToString()).JoinString(SemVersionLexer.VersionSeparator.ToString()).RemovePrefix(SemVersionLexer.VersionSeparator);

        private static IEnumerable<SemVersionIdentifier> ParsePreRelease(string? preRelease)
        {
            return preRelease?.Split(SemVersionLexer.VersionSeparator).Select(i => new SemVersionIdentifier(i)) ?? [];
        }

        public static bool operator ==(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            return !(a == b);
        }
        public static bool operator <(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            if (ReferenceEquals(null, a) || string.IsNullOrWhiteSpace(a.HashBase))
                return false;//a can only be higher or same as b when b is not set

            if (ReferenceEquals(null, b) || string.IsNullOrWhiteSpace(b.HashBase))
                return true;//a is always lower if a is set and b is not

            for (var skip = 0; skip < a.Identifiers.Count; skip++)
            {
                var aIdentifier = a.Identifiers.Skip(skip).FirstOrDefault();
                var bIdentifier = b.Identifiers.Skip(skip).FirstOrDefault();

                if (aIdentifier == bIdentifier)
                    continue;

                return aIdentifier < bIdentifier;
            }
            return a.Identifiers.Count < b.Identifiers.Count;
        }
        public static bool operator >(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            if (ReferenceEquals(null, b) || string.IsNullOrWhiteSpace(b.HashBase))
                return false;//a can only be lower or same as b when a is not set
            if (ReferenceEquals(null, a) || string.IsNullOrWhiteSpace(a.HashBase))
                return true;//a is always higher if b is set and a is not

            for (var skip = 0; skip < a.Identifiers.Count; skip++)
            {
                var aIdentifier = a.Identifiers.Skip(skip).FirstOrDefault();
                var bIdentifier = b.Identifiers.Skip(skip).FirstOrDefault();

                if (aIdentifier == bIdentifier)
                    continue;

                return aIdentifier > bIdentifier;
            }
            return a.Identifiers.Count > b.Identifiers.Count;
        }
        protected bool Equals(SemVersionPreRelease other)
        {
            return other.HashBase.Equals(HashBase);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SemVersionPreRelease)obj);
        }

        public override int GetHashCode()
        {
            return HashBase.GetHashCode();
        }

        public int CompareTo(SemVersionPreRelease other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }

        public override string ToString()
        {
            return HashBase;
        }
    }
}
