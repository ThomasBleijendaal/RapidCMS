using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Common.Extensions
{
    public static class EnumerableExtensions
    {
        //// this method cannot handle gaps in any shape or form
        //public static IEnumerable<IEnumerable<TValue>> Pivot<TValue>(this IEnumerable<IEnumerable<TValue>> source)
        //{
        //    var data = source
        //        .Select(col => col.Select((value, index) => (value, index)))
        //        .SelectMany(x => x)
        //        .GroupBy(x => x.index)
        //        .Select(row => row.Select(x => x.value));

        //    return data;
        //}

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

        //public static IEnumerable<IGrouping<TKey, TElement>> ChunckedGroupBy<TElement, TKey>(this IEnumerable<TElement> source, Func<TElement, TKey> selector)
        //{
        //    if (!source?.Any() ?? false)
        //    {
        //        yield return default(Group<TKey, TElement>);
        //        yield break;
        //    }

        //    var key = selector(source.First());
        //    var chunk = new List<TElement>();

        //    foreach (var element in source)
        //    {
        //        if (!selector(element).Equals(key))
        //        {
        //            yield return new Group<TKey, TElement>(key, chunk);
        //            chunk = new List<TElement>();
        //        }

        //        chunk.Add(element);
        //    }

        //    yield return new Group<TKey, TElement>(key, chunk);
        //}

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

    public static class AsyncEnumerableExtensions
    {
        public static async IAsyncEnumerable<TSource> WhereAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> asyncPredicate)
        {
            foreach (var element in source)
            {
                if (await asyncPredicate(element))
                {
                    yield return element;
                }
            }
        }

        public static async Task<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source)
        {
            var result = new List<TSource>();

            await foreach (var element in source)
            {
                result.Add(element);
            }

            return result;
        }

        public static async Task<List<TResult>> ToListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var result = new List<TResult>();

            await foreach (var element in source)
            {
                result.Add(selector(element));
            }

            return result;
        }
    }
}
