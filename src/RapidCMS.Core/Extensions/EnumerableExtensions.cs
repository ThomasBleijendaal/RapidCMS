using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList();
        }

        public static bool In<TElement>(this TElement source, params TElement[] list)
        {
            return list?.Contains(source) ?? false;
        }

        public static async Task<List<TResult>> ToListAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> asyncSelector)
        {
            var result = new List<TResult>();

            foreach (var element in source.Select(asyncSelector))
            {
                result.Add(await element);
            }

            return result;
        }

        public static async Task<List<TResult>> ToListAsync<TResult>(this IEnumerable<Task<TResult>> source)
        {
            var result = new List<TResult>();

            foreach (var element in source)
            {
                result.Add(await element);
            }

            return result;
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var element in source)
            {
                action.Invoke(element);
            }
        }

        public static IEnumerable<TValue> GetCommonValues<TKey, TValue>(this IDictionary<TKey, IEnumerable<TValue>> dictionary, IEqualityComparer<TValue> equalityComparer)
            where TKey : notnull
        {
            return dictionary.SelectMany(x => x.Value).Where(x => dictionary.Values.All(v => v.Contains(x, equalityComparer))).Distinct(equalityComparer);
        }

        public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> conditionalCast)
            where TResult : class
        {
            foreach (var element in source)
            {
                if (conditionalCast.Invoke(element) is TResult result)
                {
                    yield return result;
                }
            }
        }

        public static async IAsyncEnumerable<TResult> SelectNotNullAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult?>> conditionalCast)
            where TResult : class
        {
            foreach (var element in source)
            {
                if (await conditionalCast.Invoke(element) is TResult result)
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<T> MergeAll<T>(params IEnumerable<T>?[] sources)
            where T : class
        {
            return sources.Where(x => x != null).SelectMany(x => x);
        }

        public static IEnumerable<TResult> RecursiveSelect<TSource, TSeed, TResult>(this IEnumerable<TSource> source, TSeed seed, Func<TSource, TSeed, (TSeed, TResult)> walkFunction)
        {
            var previous = seed;
            foreach (var element in source)
            {
                var (newPrevious, result) = walkFunction.Invoke(element, previous);
                previous = newPrevious;
                yield return result;
            }
        }

        public static T? GetTypeFromList<T>(this IEnumerable<object> elements)
            where T : class
        {
            return elements
                .FirstOrDefault(x => x.GetType().GetInterfaces().Any(i => i == typeof(T))) as T;
        }

        public class Group<TKey, TElement> : IGrouping<TKey, TElement>
        {
            public Group(TKey key, List<TElement> elements)
            {
                Key = key;
                Elements = elements ?? throw new ArgumentNullException(nameof(elements));
            }

            public TKey Key { get; private set; }
            public List<TElement> Elements { get; set; }

            public IEnumerator<TElement> GetEnumerator()
            {
                return Elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Elements.GetEnumerator();
            }
        }
    }
}
