using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Common.Extensions
{
    public static class EnumerableExtensions
    {
        // this method cannot handle gaps in any shape or form
        public static IEnumerable<IEnumerable<TValue>> Pivot<TValue>(this IEnumerable<IEnumerable<TValue>> source)
        {
            var data = source
                .Select(col => col.Select((value, index) => (value, index)))
                .SelectMany(x => x)
                .GroupBy(x => x.index)
                .Select(row => row.Select(x => x.value));

            return data;
        }

        public static List<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList();
        }

        public static bool In<TElement>(this TElement source, params TElement[] list)
        {
            return list?.Contains(source) ?? false;
        }
    }

    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source)
        {
            var result = new List<TSource>();

            await foreach (var element in source)
            {
                result.Add(element);
            }

            return result;
        }
    }
}
