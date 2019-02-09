using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidCMS.Common.Helpers
{
    public static class EnumHelper
    {
        public static IEnumerable<TEnum> GetValues<TEnum>()
            where TEnum: Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
    }
}
