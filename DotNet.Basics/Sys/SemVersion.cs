using System;
using static System.String;

namespace DotNet.Basics.Sys
{
    public record SemVersion : IComparable<SemVersion>, IComparable
    {
        // Positional primary constructor — only the comparing fields. Metadata is `init` only.
        public SemVersion(int Major, int Minor, int Patch, SemVersionPreRelease PreRelease)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Patch = Patch;
            this.PreRelease = PreRelease;
        }

        public int Major { get; init; }
        public int Minor { get; init; }
        public int Patch { get; init; }
        public SemVersionPreRelease PreRelease { get; init; }
        public string Metadata { get; init; } = Empty;

        public SemVersion()
            : this(0, 0, 0, new SemVersionPreRelease())
        { }

        public SemVersion(object? semVer)
            : this(semVer?.ToString())
        { }

        public SemVersion(string? semVer)
        {
            var parsed = ParseInternal(semVer ?? "0.0.0");
            Major = parsed.Major;
            Minor = parsed.Minor;
            Patch = parsed.Patch;
            PreRelease = parsed.PreRelease;
            Metadata = parsed.Metadata;
        }

        public SemVersion(string? version, string? preRelease = null, string? metadata = null)
        {
            var parsed = ParseInternal(version);
            Major = parsed.Major;
            Minor = parsed.Minor;
            Patch = parsed.Patch;
            PreRelease = new SemVersionPreRelease(preRelease);
            Metadata = metadata ?? Empty;
        }

        public SemVersion(int major, int minor, int patch, string? preRelease = null, string? metadata = null)
            : this(major, minor, patch, new SemVersionPreRelease(preRelease))
        {
            Metadata = metadata ?? Empty;
        }

        public SemVersion(int major, int minor, int patch, SemVersionPreRelease? preRelease, string? metadata)
            : this(major, minor, patch, preRelease ?? new SemVersionPreRelease())
        {
            Metadata = metadata ?? Empty;
        }

        public string FileVerString => $"{Major}.{Minor}.{Patch}";

        public string SemVer10String
        {
            get
            {
                var s = FileVerString;
                if (PreRelease.Any)
                    s += $"{SemVersionLexer.PreReleaseSeparator}{PreRelease}";
                return s;
            }
        }

        public string SemVer20String
        {
            get
            {
                var s = SemVer10String;
                if (IsNullOrWhiteSpace(Metadata) == false)
                    s += $"{SemVersionLexer.MetadataSeparator}{Metadata.TrimStart(SemVersionLexer.MetadataSeparator)}";
                return s;
            }
        }

        public static SemVersion Parse(string? semVer = "0.0.0")
        {
            if (semVer == null)
                return new SemVersion();
            return ParseInternal(semVer);
        }

        private static SemVersion ParseInternal(string? semVer)
        {
            if (semVer == null)
                return new SemVersion();

            semVer = semVer.RemovePrefix("v");
            var lexer = new SemVersionLexer();
            var tokens = lexer.Lex(semVer);
            var major = int.Parse(tokens[0]);
            var minor = int.Parse(tokens[1]);
            var patch = int.Parse(tokens[2]);
            var preRelease = new SemVersionPreRelease(tokens[3]);
            var metadata = tokens[4];
            return new SemVersion(major, minor, patch, preRelease) { Metadata = metadata };
        }

        public override string ToString()
        {
            return SemVer20String;
        }

        // SemVer 2.0 §10: build metadata MUST be ignored when determining equality.
        // We override the record-generated equality to exclude Metadata.
        public virtual bool Equals(SemVersion? other)
        {
            if (other is null) return false;
            return Major == other.Major
                   && Minor == other.Minor
                   && Patch == other.Patch
                   && PreRelease == other.PreRelease;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                hashCode = (hashCode * 397) ^ PreRelease.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator <(SemVersion a, SemVersion b)
        {
            if (a.Major < b.Major) return true;
            if (a.Major > b.Major) return false;

            if (a.Minor < b.Minor) return true;
            if (a.Minor > b.Minor) return false;

            if (a.Patch < b.Patch) return true;
            if (a.Patch > b.Patch) return false;

            return a.PreRelease < b.PreRelease;
        }

        public static bool operator >(SemVersion a, SemVersion b)
        {
            if (a.Major > b.Major) return true;
            if (a.Major < b.Major) return false;

            if (a.Minor > b.Minor) return true;
            if (a.Minor < b.Minor) return false;

            if (a.Patch > b.Patch) return true;
            if (a.Patch < b.Patch) return false;

            return a.PreRelease > b.PreRelease;
        }

        public int CompareTo(SemVersion? other)
        {
            if (other is null) return 1;
            if (this < other) return -1;
            if (this > other) return 1;
            return 0;
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is SemVersion other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(SemVersion)}", nameof(obj));
        }

        public static implicit operator SemVersion(string s) => new(s);
        public static implicit operator string(SemVersion v) => v.SemVer20String;
    }
}
