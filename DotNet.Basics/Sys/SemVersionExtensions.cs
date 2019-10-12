namespace DotNet.Basics.Sys
{
    public static class SemVersionExtensions
    {
        public static SemVersion ToSemVersion(this string version)
        {
            return new SemVersion(version);
        }
    }
}
