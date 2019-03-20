namespace DotNet.Basics.Sys
{
    public class SemVersion
    {
        public SemVersion(int major, int minor, int patch, string preRelease = null, string metadata = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease ?? string.Empty;
            Metadata = metadata ?? string.Empty;
            
            //set semver10 string
            SemVer10String = $"{Major}.{Minor}.{Patch}";
            
            //set semver20 string
            SemVer20String = SemVer10String;
            if (string.IsNullOrWhiteSpace(PreRelease) == false)
                SemVer20String += $"{SemVersionLexer.PreReleaseSeparator}{PreRelease.TrimStart(SemVersionLexer.PreReleaseSeparator)}";
            if (string.IsNullOrWhiteSpace(Metadata) == false)
                SemVer20String += $"{SemVersionLexer.MetadataSeparator}{Metadata.TrimStart(SemVersionLexer.MetadataSeparator)}";
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string PreRelease { get; }
        public string Metadata { get; }

        public string SemVer10String { get; }
        public string SemVer20String { get; }
        
        public static SemVersion Parse(string semVer)
        {
            var lexer = new SemVersionLexer();
            var tokens = lexer.Lex(semVer);
            var major = int.Parse(tokens[0]);
            var minor = int.Parse(tokens[1]);
            var patch = int.Parse(tokens[2]);
            var preRelease = tokens[3];
            var metaData = tokens[4];
            return new SemVersion(major, minor, patch, preRelease, metaData);
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
