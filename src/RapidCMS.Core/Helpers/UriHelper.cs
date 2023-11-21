using System.Linq;
using System.Web;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Helpers;

internal static class UriHelper
{
    public static string CombinePath(params string?[] elements)
        => string.Join("/", elements.SelectNotNull(x => x));

    public static string CombineQueryString(params (string key, string? value)[] elements)
        => string.Join("&", elements
                .Where(x => !string.IsNullOrWhiteSpace(x.value))
                .Select(x => $"{x.key}={HttpUtility.UrlEncode(x.value)}"))
            is string query && !string.IsNullOrWhiteSpace(query)
            ? $"?{query}"
            : "";
}
