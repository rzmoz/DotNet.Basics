using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Collections
{
    public static class CollectionExtensions
    {
        public static async Task ParallelForEachAsync<T>(this IEnumerable<T> col, Func<T, CancellationToken, Task> forEachAction, CancellationToken ct = default(CancellationToken))
        {
            var tasks = new List<Task>();

            foreach (var item in col)
                tasks.Add(forEachAction(item, ct));

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
