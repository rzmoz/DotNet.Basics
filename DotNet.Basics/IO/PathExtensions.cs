using System;
using System.Linq;

namespace DotNet.Basics.IO
{
    public static class PathExtensions
    {
        public static PathDelimiter ToPathDelimiter(this char delimiter)
        {
            switch (delimiter)
            {
                case PathChars.Backslash:
                    return PathDelimiter.Backslash;
                case PathChars.Slash:
                    return PathDelimiter.Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {delimiter}");
            }
        }

        public static char ToChar(this PathDelimiter pathDelimiter)
        {
            switch (pathDelimiter)
            {
                case PathDelimiter.Backslash:
                    return PathChars.Backslash;
                case PathDelimiter.Slash:
                    return PathChars.Slash;
                default:
                    throw new NotSupportedException($"Path delimiter not supported: {pathDelimiter}");
            }
        }

        public static bool IsProtocol(this string path)
        {
            return path?.EndsWith("://") ?? false;
        }
    }
}
