using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Collections
{
    public static class CollectionExtensions
    {
        public static async Task ParallelForEachAsync<T>(this IEnumerable<T> col, Func<T, Task> forEachAction)
        {
            var tasks = col.ForEach(forEachAction);
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public static IEnumerable<TK> ForEach<T, TK>(this IEnumerable<T> col, Func<T, TK> forEachAction)
        {
            return col.Select(forEachAction).ToList();//Invoke ToList() is needed in order to invoke the action
        }

        public static void ForEach<T>(this IEnumerable<T> col, Action<T> forEachAction)
        {
            foreach (var item in col)
                forEachAction(item);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T t)
        {
            return new[] { t };
        }

    }
}
