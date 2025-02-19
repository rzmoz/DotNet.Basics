using System.Collections.Generic;

namespace DotNet.Basics.Cli
{
    public interface IArgsParser
    {
        ArgsDictionary Parse(params IEnumerable<string> args);
    }
}
