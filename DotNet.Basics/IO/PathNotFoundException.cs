using System;

namespace DotNet.Basics.IO
{
    public class PathNotFoundException : Exception
    {
        public PathNotFoundException(PathInfo path) : base($"{path?.FullName}")
        {
            Path = path;
        }

        public PathInfo Path { get; }
    }
}
