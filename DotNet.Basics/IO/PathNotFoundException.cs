using System;

namespace DotNet.Basics.IO
{
    public class PathNotFoundException : Exception
    {
        public PathNotFoundException(Path path) : base($"{path?.FullName}")
        {
            Path = path;
        }

        public Path Path { get; }
    }
}
