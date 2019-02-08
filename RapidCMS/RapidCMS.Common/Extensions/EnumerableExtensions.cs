using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
