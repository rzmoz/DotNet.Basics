using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.IO
{
    public class PathException : Exception
    {
        public PathException(Path path)
            : this($"Path not found: {path?.FullName}", path)
        {
        }

        public PathException(string message, Path path) : base($"{message} :{path?.FullName}")
        {
            Path = path;
        }

        public PathException(string message, Exception innerException, Path path) : base($"{message} :{path?.FullName}", innerException)
        {
            Path = path;
        }

        protected PathException(SerializationInfo info, StreamingContext context, Path path) : base(info, context)
        {
            Path = path;
        }

        public Path Path { get; }
    }
}
