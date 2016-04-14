using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> col, Func<T, T> forEachAction)
        {
            return col.Select(forEachAction).ToList();//to list is important to run in order to invoke the action
        }
    }
}
