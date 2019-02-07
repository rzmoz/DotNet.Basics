using System;

namespace DotNet.Basics.Sys
{
    public class SemVersion
    {
        public SemVersion(string semVerString)
        {
            var semVer = Parse(semVerString);
            Major = semVer.Major;
            Minor = semVer.Minor;
            Patch = semVer.Patch;
            PreRelease = semVer.PreRelease;
            Metadata = semVer.Metadata;
        }

        public SemVersion(uint major, uint minor, uint patch, string preRelease = null, string metadata = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease ?? string.Empty;
            Metadata = metadata ?? string.Empty;
        }

        public uint Major { get; }
        public uint Minor { get; }
        public uint Patch { get; }
        public string PreRelease { get; }
        public string Metadata { get; }

        public static SemVersion Parse(string semVer)
        {
            return new SemVersion(0, 0, 0);
        }

        public static bool operator <(SemVersion a, SemVersion b)
        {
            if (a.Major < b.Major)
                return true;
            if (a.Minor < b.Minor)
                return true;
            if (a.Patch < b.Patch)
                return true;
            return false;
        }
        public static bool operator >(SemVersion a, SemVersion b)
        {
            if (a.Major > b.Major)
                return true;
            if (a.Minor > b.Minor)
                return true;
            if (a.Patch > b.Patch)
                return true;
            return false;
        }
    }
}
