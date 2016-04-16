using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Collections
{
    public static class CollectionExtensions
    {
        public static IEnumerable<TK> ForEach<T, TK>(this IEnumerable<T> col, Func<T, TK> forEachAction)
        {
            return col.Select(forEachAction).ToList();//Invoke ToList() is needed in order to invoke the action
        }

        public static void ForEach<T>(this IEnumerable<T> col, Action<T> forEachAction)
        {
            foreach (var item in col)
                forEachAction(item);
        }
    }
}
