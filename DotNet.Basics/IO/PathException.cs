using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.IO
{
    public class PathException : Exception
    {
        public PathException(PathInfo path)
            : this($"Path not found: {path?.FullName}", path)
        {
        }

        public PathException(string message, PathInfo path) : base($"{message} :{path?.FullName}")
        {
            Path = path;
        }

        public PathException(string message, Exception innerException, PathInfo path) : base($"{message} :{path?.FullName}", innerException)
        {
            Path = path;
        }

        protected PathException(SerializationInfo info, StreamingContext context, PathInfo path) : base(info, context)
        {
            Path = path;
        }

        public PathInfo Path { get; }
    }
}
