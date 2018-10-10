using System.Collections.Generic;

namespace DotNet.Basics.Collections
{
    public interface IMultiLevelDictionary<T> : IDictionary<string, T>
    {
        char PathSeparator { get; }
    }
}
