using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
