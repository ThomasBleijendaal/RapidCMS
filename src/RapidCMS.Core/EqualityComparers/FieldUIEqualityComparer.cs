using System.Collections.Generic;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.EqualityComparers
{
    internal class FieldUIEqualityComparer : IEqualityComparer<FieldUI>
    {
        public bool Equals(FieldUI? x, FieldUI? y)
        {
            return x?.Name?.Equals(y?.Name) ?? false;
        }

        public int GetHashCode(FieldUI obj)
        {
            return obj.Name?.GetHashCode() ?? default;
        }
    }
}
