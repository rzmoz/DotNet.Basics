using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Basics.Collections
{
    public static class CollectionExtensions
    {
        public static string JoinString(this IEnumerable<string> source, string separator = "|")
        {
            return string.Join(separator, source);
        }

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

        public static Task ForEachParallelAsync<T>(this IEnumerable<T> col, Func<T, Task> forEachAction)
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
