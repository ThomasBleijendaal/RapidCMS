using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.EqualityComparers
{
    internal class FieldUIEqualityComparer : IEqualityComparer<FieldUI>
    {
        public bool Equals(FieldUI x, FieldUI y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(FieldUI obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
