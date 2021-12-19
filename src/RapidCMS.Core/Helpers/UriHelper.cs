using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Helpers
{
    internal static class UriHelper
    {
        public static string Combine(params string?[] elements)
            => string.Join("/", elements.SelectNotNull(x => x));
    }
}
