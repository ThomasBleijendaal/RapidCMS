using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Extensions
{
    internal static class EnumExtensions
    {
        public static TAttribute? GetCustomAttribute<TAttribute>(this Enum e)
            where TAttribute : Attribute
        {
            var member = e.GetType().GetMember(e.ToString()).FirstOrDefault();

            if (member != null)
            {
                var attribute = member
                    .GetCustomAttributes(typeof(TAttribute), true)
                    ?.FirstOrDefault();
                ;

                return attribute as TAttribute;
            }

            return default;
        }
    }
}
