using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNet.Basics.Collections
{
    public static class EnumerableStringExtensions
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

        public static ICollection<T> ForEachParallel<T>(this IEnumerable<T> col, Action<T> forEachAction)
        {
            if (forEachAction == null) throw new ArgumentNullException(nameof(forEachAction));
            var list = col.ToList();
            Parallel.ForEach(list, forEachAction);
            return list;
        }
        public static ICollection<TK> ForEachParallel<T, TK>(this IEnumerable<T> col, Func<T, TK> forEachFunc)
        {
            if (forEachFunc == null) throw new ArgumentNullException(nameof(forEachFunc));
            var results = new ConcurrentStack<TK>();
            Parallel.ForEach(col, item => results.Push(forEachFunc.Invoke(item)));
            return results.ToList();
        }

        public static async Task<ICollection<T>> ForEachParallelAsync<T>(this IEnumerable<T> col, Func<T, Task> forEachAction)
        {
            var list = col.ToList();
            if (forEachAction == null)
                return list;
            await Task.WhenAll(list.Select(forEachAction)).ConfigureAwait(false);
            return list;
        }

        public static Task<TK[]> ForEachParallelAsync<T, TK>(this IEnumerable<T> col, Func<T, Task<TK>> forEachFunc)
        {
            if (forEachFunc == null) throw new ArgumentNullException(nameof(forEachFunc));
            return Task.WhenAll(col.Select(forEachFunc));
        }

        public static ICollection<T> ForEach<T>(this IEnumerable<T> col, Action<T> forEachAction)
        {
            if (forEachAction == null) throw new ArgumentNullException(nameof(forEachAction));

            var list = col.ToList();
            foreach (var item in list)
                forEachAction.Invoke(item);
            return list;
        }

        public static ICollection<TK> ForEach<T, TK>(this IEnumerable<T> col, Func<T, TK> forEachFunc)
        {
            if (forEachFunc == null) throw new ArgumentNullException(nameof(forEachFunc));

            var results = new List<TK>();

            foreach (var item in col)
                results.Add(forEachFunc.Invoke(item));
            return results;
        }

        public static async Task<ICollection<T>> ForEachAsync<T>(this IEnumerable<T> col, Func<T, Task> forEachAction)
        {
            if (forEachAction == null) throw new ArgumentNullException(nameof(forEachAction));

            var list = col.ToList();

            foreach (var item in list)
                await forEachAction.Invoke(item).ConfigureAwait(false);
            return list;
        }

        public static async Task<ICollection<TK>> ForEachAsync<T, TK>(this IEnumerable<T> col, Func<T, Task<TK>> forEachFunc)
        {
            if (forEachFunc == null) throw new ArgumentNullException(nameof(forEachFunc));

            var results = new List<TK>();

            foreach (var item in col)
            {
                var result = await forEachFunc.Invoke(item).ConfigureAwait(false);
                results.Add(result);
            }
            return results;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> t, IEnumerable<T> append)
        {
            return [.. t, .. append];
        }

        public static IEnumerable<string> Whitelist(this IEnumerable<string> all, Regex regex)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));
            return all.Where(element => regex.IsMatch(element));
        }

        public static IEnumerable<string> Whitelist<T>(this T all, params string[] whitelist) where T : IEnumerable<string>
        {
            return all.Whitelist(StringComparison.CurrentCultureIgnoreCase, whitelist);
        }

        public static IEnumerable<string> Whitelist(this IEnumerable<string> all, StringComparison comparison, params string[] whitelist)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));

            foreach (var element in all)
            {
                if (whitelist.Any(incl => element.Equals(incl, comparison)))
                    yield return element;
            }
        }
        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, Regex regex)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));
            return all.Where(element => !regex.IsMatch(element));
        }

        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, params string[] blacklist)
        {
            return all.Blacklist(StringComparison.CurrentCultureIgnoreCase, blacklist);
        }
        public static IEnumerable<string> Blacklist(this IEnumerable<string> all, StringComparison comparison, params string[] blacklist)
        {
            if (all == null) throw new ArgumentNullException(nameof(all));

            foreach (var element in all)
            {
                if (blacklist.Any(incl => element.Equals(incl, comparison)))
                    continue;
                yield return element;
            }
        }
    }
}
