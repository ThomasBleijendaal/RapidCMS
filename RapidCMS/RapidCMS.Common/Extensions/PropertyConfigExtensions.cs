using System;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class PropertyConfigExtensions
    {
        [Obsolete]
        public static Field ToField(this PropertyConfig property)
        {
            return new ExpressionField(property.Property)
            {
                Index = property.Index,

                Description = property.Description,
                Name = property.Name,
                Expression = property.Property,

                Readonly = true,
                IsVisible = property.IsVisible
            };
        }
    }
}
