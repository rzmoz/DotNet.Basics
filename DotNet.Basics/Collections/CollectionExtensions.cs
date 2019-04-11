using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Basics.Collections
{
    public static class CollectionExtensions
    {
        public static bool None<TSource>(this IEnumerable<TSource> source)
        {
            return source.Any() == false;
        }
        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Any(predicate) == false;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Any(predicate);
        }

        public static void ForEachParallel<T>(this IEnumerable<T> col, Action<T> forEachAction)
        {
            Parallel.ForEach(col, forEachAction);
        }
        public static TK[] ForEachParallel<T, TK>(this IEnumerable<T> col, Func<T, TK> forEachAction)
        {
            var results = new ConcurrentStack<TK>();
            Parallel.ForEach(col, item => results.Push(forEachAction.Invoke(item)));
            return results.ToArray();
        }

        public static Task ForEachParallelAsync<T>(this IEnumerable<T> col, Func<T, Task> forEachAction)
        {
            return Task.WhenAll(col.Select(forEachAction));
        }
        public static Task<TK[]> ForEachParallelAsync<T, TK>(this IEnumerable<T> col, Func<T, Task<TK>> forEachAction)
        {
            return Task.WhenAll(col.Select(forEachAction));
        }

        public static void ForEach<T>(this IEnumerable<T> col, Action<T> forEachAction)
        {
            foreach (var item in col)
                forEachAction?.Invoke(item);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T t)
        {
            yield return t;
        }
        public static IEnumerable<T> ToEnumerable<T>(this T t, IEnumerable<T> concat)
        {
            return t.ToEnumerable().Concat(concat);
        }
    }
}
