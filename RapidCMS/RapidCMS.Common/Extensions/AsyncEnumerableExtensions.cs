using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Common.Extensions
{
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
